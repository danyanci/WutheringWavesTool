//委托
using System.Buffers;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using SqlSugar;
using Waves.Api.Models;
using Waves.Core.Common;
using Waves.Core.Models;
using Waves.Core.Models.Downloader;
using Waves.Core.Models.Enums;

namespace Waves.Core.GameContext;

public partial class GameContextBase
{
    #region 常量

    const int MaxBufferSize = 65536; // 64KB缓冲区
    const long UpdateThreshold = 1048576; // 1MB进度更新阈值
    #endregion

    #region 字段和属性
    private string _downloadBaseUrl;
    private long _totalfileSize = 0L;
    private long _totalProgressSize = 0L;
    #endregion

    #region DownloadStatus
    private long _totalVerifiedBytes;
    private long _totalDownloadedBytes;
    private DateTime _lastSpeedUpdateTime;
    private double _downloadSpeed;
    private double _verifySpeed;

    private DateTime _lastSpeedTime = DateTime.Now;
    private long _lastSpeedBytes; // 速度计算基准值
    #endregion

    #region 速度属性
    public double DownloadSpeed => _downloadSpeed;
    public double VerifySpeed => _verifySpeed;
    #endregion
    #region DownloadStatus
    private DownloadState? _downloadState;
    #endregion
    #region 公开方法
    public async Task StartDownloadTaskAsync(string folder, GameLauncherSource source)
    {
        if (source == null || string.IsNullOrWhiteSpace(folder))
            return;
        _downloadCTS = new CancellationTokenSource();
        _isDownload = true;
        _totalProgressSize = 0;
        await GameLocalConfig.SaveConfigAsync(GameLocalSettingName.GameLauncherBassFolder, folder);

        await GetGameResourceAsync(folder, source);
    }
    #endregion

    #region 核心下载逻辑
    private async Task GetGameResourceAsync(string folder, GameLauncherSource source)
    {
        try
        {
            var resource = await GetGameResourceAsync(source.ResourceDefault);
            if (resource == null)
                return;

            // 构建下载基础URL
            _downloadBaseUrl =
                source.ResourceDefault.CdnList.Where(x => x.P != 0).OrderBy(x => x.P).First().Url
                + source.ResourceDefault.Config.BaseUrl;

            HttpClientService.BuildClient();
            await InitializeProgress(resource);
            Task.Run(() => StartDownloadAsync(folder, resource));
        }
        catch (IOException ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    private async Task StartDownloadAsync(string folder, IndexGameResource resource)
    {
        try
        {
            _downloadState.IsActive = true;
            await UpdateFileProgress(GameContextActionType.Verify, 0);
            foreach (var file in resource.Resource)
            {
                if (_downloadCTS?.IsCancellationRequested ?? true)
                {
                    this._downloadState.IsActive = false;
                    await SetNoneStatusAsync().ConfigureAwait(false);
                    return;
                }
                var filePath = BuildFilePath(folder, file);
                if (File.Exists(filePath))
                {
                    if (file.ChunkInfos == null)
                    {
                        var checkResult = await VaildateFullFile(file, filePath);
                        if (checkResult)
                        {
                            await DownloadFileByFull(
                                file,
                                filePath,
                                new()
                                {
                                    Start = 0,
                                    End = file.Size - 1,
                                    Md5 = file.Md5,
                                }
                            );
                            //await FinalValidation(file, filePath);
                        }
                    }
                    else
                    {
                        var fileName = System.IO.Path.GetFileName(filePath);
                        for (int i = 0; i < file.ChunkInfos.Count; i++)
                        {
                            var needDownload = await ValidateFileChunks(
                                file.ChunkInfos[i],
                                filePath
                            );
                            if (needDownload)
                            {
                                if (i == file.ChunkInfos.Count - 1)
                                {
                                    HttpClientService.BuildClient();
                                    await DownloadFileByChunks(
                                        file,
                                        filePath,
                                        file.ChunkInfos[i],
                                        true,
                                        file.Size
                                    );
                                }
                                else
                                {
                                    HttpClientService.BuildClient();
                                    await DownloadFileByChunks(
                                        file,
                                        filePath,
                                        file.ChunkInfos[i],
                                        false
                                    );
                                }
                            }
                        }
                    }
                    //await FinalValidation(file, filePath);
                }
                else
                {
                    await DownloadFileByFull(
                        file,
                        filePath,
                        new IndexChunkInfo()
                        {
                            Start = 0,
                            End = file.Size - 1,
                            Md5 = file.Md5,
                        }
                    );
                    //await FinalValidation(file, filePath);
                }
            }
            _downloadState.IsActive = false;
        }
        catch (IOException ex)
        {
            Debug.WriteLine(ex.Message);
        }
        catch (OperationCanceledException)
        {
            _downloadState.IsActive = false;
            await SetNoneStatusAsync().ConfigureAwait(false);
        }
        finally
        {
            _downloadCTS.Dispose();
            _downloadCTS = null;
        }
    }

    public async Task<bool> PauseDownloadAsync()
    {
        if (this._downloadState.IsActive)
        {
            return await this._downloadState.PauseAsync();
        }

        return false;
    }

    public async Task<bool> ResumeDownloadAsync()
    {
        if (_downloadState.IsPaused)
        {
            _lastSpeedTime = DateTime.Now;
            return await _downloadState.ResumeAsync();
        }
        return false;
    }

    public async Task<bool> StopDownloadAsync()
    {
        try
        {
            if (_downloadCTS != null && !_downloadCTS.IsCancellationRequested)
            {
                await _downloadCTS.CancelAsync().ConfigureAwait(false);
            }

            Interlocked.Exchange(ref _totalProgressSize, 0L);
            Interlocked.Exchange(ref _totalfileSize, 0L);
            Interlocked.Exchange(ref _totalVerifiedBytes, 0L);
            Interlocked.Exchange(ref _totalDownloadedBytes, 0L);
            _downloadCTS?.Dispose();

            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"取消下载异常: {ex.Message}");
            return false;
        }
    }
    #endregion

    #region 校验逻辑
    /// <summary>
    /// 校验
    /// </summary>
    /// <param name="file"></param>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private async Task<bool> ValidateFileChunks(IndexChunkInfo file, string filePath)
    {
        using (
            var fs = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                262144,
                true
            )
        )
        {
            try
            {
                if (fs.Length < file.End + 1) // 检查文件长度是否足够
                {
                    Debug.WriteLine($"文件长度不足: {fs.Length} < {file.End + 1}");
                    return true;
                }
                var memoryPool = ArrayPool<byte>.Shared;
                long offset = file.Start;
                long remaining = file.End - file.Start + 1;
                bool isValid = true;
                fs.Seek(offset, SeekOrigin.Begin);
                using (var md5 = MD5.Create())
                {
                    long accumulatedBytes = 0L;
                    while (remaining > 0 && isValid)
                    {
                        await this._downloadState.PauseToken.WaitIfPausedAsync();
                        var buffer = memoryPool.Rent(MaxBufferSize);
                        try
                        {
                            int bytesRead = await fs.ReadAsync(buffer, 0, MaxBufferSize)
                                .ConfigureAwait(false);
                            if (bytesRead == 0)
                            {
                                isValid = false;
                                break;
                            }
                            md5.TransformBlock(buffer, 0, bytesRead, null, 0);
                            remaining -= bytesRead;
                            accumulatedBytes += bytesRead;
                            if (accumulatedBytes >= UpdateThreshold)
                            {
                                await UpdateFileProgress(
                                        GameContextActionType.Verify,
                                        accumulatedBytes
                                    )
                                    .ConfigureAwait(false);
                                accumulatedBytes = 0;
                            }
                        }
                        catch (IOException ex) { }
                        finally
                        {
                            memoryPool.Return(buffer);
                        }
                    }
                    if (accumulatedBytes > 0 && accumulatedBytes < UpdateThreshold)
                    {
                        await UpdateFileProgress(GameContextActionType.Verify, accumulatedBytes)
                            .ConfigureAwait(false);
                    }
                    md5.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
                    string hash = BitConverter.ToString(md5.Hash!).Replace("-", "").ToLower();
                    isValid = hash == file.Md5.ToLower();
                    return !isValid;
                }
            }
            catch (IOException ex)
            {
                return false;
            }
        }
    }

    private async Task<bool> VaildateFullFile(IndexResource file, string filePath)
    {
        const int bufferSize = 262144; // 80KB缓冲区
        using var md5 = MD5.Create();
        var memoryPool = ArrayPool<byte>.Shared;
        const long UpdateThreshold = 1048576; // 1MB进度更新阈值
        try
        {
            using (
                var fs = new FileStream(
                    filePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: bufferSize,
                    true
                )
            )
            {
                long accumulatedBytes = 0L;
                while (true)
                {
                    //暂停锁
                    await this._downloadState.PauseToken.WaitIfPausedAsync();
                    byte[] buffer = memoryPool.Rent(bufferSize);
                    try
                    {
                        int bytesRead = await fs.ReadAsync(buffer.AsMemory(0, bufferSize))
                            .ConfigureAwait(false);
                        if (bytesRead == 0)
                            break;
                        md5.TransformBlock(buffer, 0, bytesRead, null, 0);
                        accumulatedBytes += bytesRead; // 添加此行以累加字节数
                        if (accumulatedBytes >= UpdateThreshold)
                        {
                            await UpdateFileProgress(GameContextActionType.Verify, accumulatedBytes)
                                .ConfigureAwait(false);
                            accumulatedBytes = 0;
                        }
                    }
                    finally
                    {
                        memoryPool.Return(buffer);
                    }
                }
                if (accumulatedBytes < UpdateThreshold)
                {
                    await UpdateFileProgress(GameContextActionType.Verify, accumulatedBytes)
                        .ConfigureAwait(false);
                }
            }

            md5.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
            string hash = BitConverter.ToString(md5.Hash!).Replace("-", "").ToLower();
            return !(hash == file.Md5);
        }
        catch (IOException ex)
        {
            return true;
        }
        catch (Exception ex)
        {
            return true;
        }
    }

    #endregion

    #region 下载逻辑
    private async Task DownloadFileByChunks(
        IndexResource file,
        string filePath,
        IndexChunkInfo chunk,
        bool isLast = false,
        long allSize = 0L
    )
    {
        try
        {
            using (
                var fileStream = new FileStream(
                    filePath,
                    FileMode.OpenOrCreate,
                    FileAccess.ReadWrite,
                    FileShare.Read,
                    262144,
                    true
                )
            )
            {
                long accumulatedBytes = 0;
                using var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    _downloadBaseUrl + file.Dest
                );
                request.Headers.Range = new RangeHeaderValue(chunk.Start, chunk.End);
                using var response = await HttpClientService.GameDownloadClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    _downloadCTS.Token
                );
                var stream = await response.Content.ReadAsStreamAsync(_downloadCTS.Token);
                try
                {
                    // 预校验分片范围
                    if (chunk.Start < 0 || chunk.End < chunk.Start)
                    {
                        throw new ArgumentException($"分片范围无效: {chunk.Start}-{chunk.End}");
                    }

                    long totalWritten = 0;
                    long chunkTotalSize = chunk.End - chunk.Start + 1;
                    var memoryPool = ArrayPool<byte>.Shared;
                    fileStream.Seek(chunk.Start, SeekOrigin.Begin);
                    while (totalWritten < chunkTotalSize)
                    {
                        await _downloadState.PauseToken.WaitIfPausedAsync().ConfigureAwait(false); // 暂停检查也异步化
                        int bytesToRead = (int)
                            Math.Min(MaxBufferSize, chunkTotalSize - totalWritten);
                        byte[] buffer = ArrayPool<byte>.Shared.Rent(bytesToRead);
                        try
                        {
                            int bytesRead = await stream
                                .ReadAsync(buffer.AsMemory(0, bytesToRead), _downloadCTS.Token)
                                .ConfigureAwait(false);
                            if (bytesRead == 0)
                                break;

                            await _downloadState
                                .SpeedLimiter.LimitAsync(bytesRead)
                                .ConfigureAwait(false);
                            await fileStream
                                .WriteAsync(buffer.AsMemory(0, bytesRead), _downloadCTS.Token)
                                .ConfigureAwait(false);
                            totalWritten += bytesRead;
                            accumulatedBytes += bytesRead;
                            if (accumulatedBytes >= UpdateThreshold)
                            {
                                await UpdateFileProgress(
                                        GameContextActionType.Download,
                                        accumulatedBytes
                                    )
                                    .ConfigureAwait(false);
                                accumulatedBytes = 0;
                            }
                        }
                        finally
                        {
                            ArrayPool<byte>.Shared.Return(buffer);
                        }
                    }
                    if (accumulatedBytes > 0)
                    {
                        await UpdateFileProgress(GameContextActionType.Download, accumulatedBytes)
                            .ConfigureAwait(false);
                    }
                    if (isLast)
                        fileStream.SetLength(allSize);
                    await fileStream.FlushAsync(_downloadCTS.Token);
                }
                catch (IOException ex)
                {
                    throw;
                }
                await fileStream.FlushAsync();
                stream.Close();
                await stream.DisposeAsync();
            }
        }
        catch (IOException ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public async Task SetNoneStatusAsync()
    {
        await this.gameContextOutputDelegate?.Invoke(
            this,
            new GameContextOutputArgs()
            {
                Type = GameContextActionType.None,
                CurrentSize = _totalProgressSize,
                TotalSize = _totalfileSize,
                DownloadSpeed = _downloadSpeed,
                VerifySpeed = VerifySpeed,
                RemainingTime = this.RemainingTime,
            }
        );
    }

    public async Task SetSpeedLimitAsync(long bytesPerSecond)
    {
        await _downloadState.SetSpeedLimitAsync(bytesPerSecond);
    }

    private async Task DownloadFileByFull(IndexResource file, string filePath, IndexChunkInfo chunk)
    {
        long accumulatedBytes = 0;

        try
        {
            using (
                var fileStream = new FileStream(
                    filePath,
                    FileMode.OpenOrCreate,
                    FileAccess.ReadWrite,
                    FileShare.None,
                    262144,
                    true
                )
            ) // 明确启用异步IO
            {
                long currentBytes = 0;
                using var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    _downloadBaseUrl + file.Dest
                );
                request.Headers.Range = new RangeHeaderValue(chunk.Start, chunk.End);

                using var response = await HttpClientService
                    .GameDownloadClient.SendAsync(
                        request,
                        HttpCompletionOption.ResponseHeadersRead,
                        _downloadCTS.Token
                    )
                    .ConfigureAwait(false); // 非UI上下文切换

                response.EnsureSuccessStatusCode();
                var stream = await response
                    .Content.ReadAsStreamAsync(_downloadCTS.Token)
                    .ConfigureAwait(false);

                try
                {
                    if (chunk.Start < 0 || chunk.End < chunk.Start)
                    {
                        throw new ArgumentException($"分片范围无效: {chunk.Start}-{chunk.End}");
                    }

                    long totalWritten = 0;
                    long chunkTotalSize = chunk.End - chunk.Start + 1;
                    var memoryPool = ArrayPool<byte>.Shared;

                    fileStream.Seek(chunk.Start, SeekOrigin.Begin);

                    while (totalWritten < chunkTotalSize)
                    {
                        await _downloadState.PauseToken.WaitIfPausedAsync().ConfigureAwait(false); // 暂停检查也异步化

                        int bytesToRead = (int)
                            Math.Min(MaxBufferSize, chunkTotalSize - totalWritten);
                        byte[] buffer = memoryPool.Rent(bytesToRead);
                        try
                        {
                            int bytesRead = await stream
                                .ReadAsync(buffer.AsMemory(0, bytesToRead), _downloadCTS.Token)
                                .ConfigureAwait(false);
                            if (bytesRead == 0)
                            {
                                throw new IOException(
                                    $"数据流提前结束，预期 {chunkTotalSize} 字节，已写入 {totalWritten} 字节"
                                );
                            }
                            await _downloadState
                                .SpeedLimiter.LimitAsync(bytesRead)
                                .ConfigureAwait(false);
                            await fileStream
                                .WriteAsync(buffer.AsMemory(0, bytesRead), _downloadCTS.Token)
                                .ConfigureAwait(false);
                            totalWritten += bytesRead;
                            accumulatedBytes += bytesRead;
                            if (accumulatedBytes >= UpdateThreshold)
                            {
                                await UpdateFileProgress(
                                        GameContextActionType.Download,
                                        accumulatedBytes
                                    )
                                    .ConfigureAwait(false);
                                accumulatedBytes = 0;
                            }
                        }
                        finally
                        {
                            memoryPool.Return(buffer);
                        }
                    }
                    if (accumulatedBytes > 0)
                    {
                        await UpdateFileProgress(GameContextActionType.Download, accumulatedBytes)
                            .ConfigureAwait(false);
                    }
                    if (totalWritten != chunkTotalSize)
                    {
                        throw new IOException($"分片写入不完整: {totalWritten}/{chunkTotalSize}");
                    }
                    fileStream.SetLength(file.Size);
                    await fileStream.FlushAsync();
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (IOException ex)
                {
                    throw;
                }
            }
        }
        catch (Exception ex)
        {
            throw; // 根据业务需求决定是否重新抛出
        }
    }
    #endregion

    #region 辅助方法

    private async Task FinalValidation(IndexResource file, string filePath)
    {
        try
        {
            using (
                var fs = new FileStream(
                    filePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    262144,
                    true
                )
            )
            {
                var fullHash = await ComputeFullHash(fs);
                if (fullHash.ToLower() != file.Md5.ToLower()) { }
                else
                {
                    await fs.FlushAsync();
                }
            }
        }
        catch (Exception ex) { }
    }

    private async Task UpdateFileProgress(GameContextActionType type, long fileSize)
    {
        if (type == GameContextActionType.Download)
        {
            Interlocked.Add(ref _totalDownloadedBytes, fileSize);
        }
        else if (type == GameContextActionType.Verify)
        {
            Interlocked.Add(ref _totalVerifiedBytes, fileSize);
        }
        Interlocked.Add(ref _totalProgressSize, fileSize);
        Debug.WriteLine($"处理字节总数:{_totalProgressSize}/{_totalfileSize}");
        var elapsed = (DateTime.Now - _lastSpeedUpdateTime).TotalSeconds;
        if (elapsed >= 1)
        {
            _downloadSpeed = _totalDownloadedBytes / elapsed;
            _verifySpeed = _totalVerifiedBytes / elapsed;
            // 重置计数器和时间
            Interlocked.Exchange(ref _totalDownloadedBytes, 0);
            Interlocked.Exchange(ref _totalVerifiedBytes, 0);
            var currentBytes = Interlocked.Read(ref _totalDownloadedBytes);
            _lastSpeedBytes = currentBytes;
            _lastSpeedUpdateTime = DateTime.Now;
        }

        if (gameContextOutputDelegate == null)
            return;
        await gameContextOutputDelegate
            .Invoke(
                this,
                new GameContextOutputArgs
                {
                    Type = type,
                    CurrentSize = _totalProgressSize,
                    TotalSize = _totalfileSize,
                    DownloadSpeed = _downloadSpeed,
                    VerifySpeed = _verifySpeed,
                    RemainingTime = RemainingTime,
                    IsAction = _downloadState?.IsActive ?? false,
                    IsPause = _downloadState?.IsPaused ?? false,
                }
            )
            .ConfigureAwait(false); // 确保不阻塞下载线程
    }

    public TimeSpan RemainingTime
    {
        get
        {
            if (DownloadSpeed <= 0 || _totalDownloadedBytes >= _totalfileSize)
                return TimeSpan.Zero;

            var remainingBytes = _totalfileSize - _totalProgressSize;
            return TimeSpan.FromSeconds(remainingBytes / DownloadSpeed);
        }
    }

    #endregion

    #region 公共辅助方法
    private static async Task<string> ComputeFullHash(Stream stream)
    {
        using var md5 = MD5.Create();
        var hashBytes = await md5.ComputeHashAsync(stream);
        return BitConverter.ToString(hashBytes).Replace("-", "");
    }

    private string BuildFilePath(string folder, IndexResource file)
    {
        var path = Path.Combine(folder, file.Dest.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        return path;
    }

    private async Task InitializeProgress(IndexGameResource resource)
    {
        _totalfileSize = resource.Resource.Sum(x => x.Size);
        this._downloadState = new DownloadState(resource);
        await gameContextOutputDelegate?.Invoke(
            this,
            new GameContextOutputArgs
            {
                CurrentSize = 0,
                TotalSize = resource.Resource.Sum(x => x.Size),
                Type = GameContextActionType.Download,
            }
        );
    }
    #endregion
}
