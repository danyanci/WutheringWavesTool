//全部分片一次性检查
using System.Buffers;
using System.Diagnostics;
using System.Net.Http;
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
    #region 字段和属性
    private SpeedLimiter _speedLimiter;
    private string _downloadBaseUrl;
    private readonly Queue<double> _speedHistory = new();
    private const int SpeedHistoryWindowSize = 5;
    private Stopwatch _globalSpeedWatch = new();
    private long _totalDownloadedBytes;
    #endregion

    #region 公开方法
    public async Task StartDownloadTaskAsync(string folder, GameLauncherSource source)
    {
        if (source == null || string.IsNullOrWhiteSpace(folder))
            return;
        _speedLimiter = new SpeedLimiter(1024 * 1024);
        _downloadCTS = new CancellationTokenSource();

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
            InitializeProgress(resource);

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
            var totalSize = resource.Resource.Sum(x => x.Size);
            var totalFiles = resource.Resource.Count;
            UpdateFileProgress(
                currentFile: 0,
                totalFiles: totalFiles,
                fileSize: 0,
                totalSize: totalSize
            );
            foreach (var file in resource.Resource)
            {
                _globalSpeedWatch.Restart();
                var filePath = BuildFilePath(folder, file);
                if (File.Exists(filePath))
                {
                    Debug.WriteLine($"开始校验文件:{filePath}");
                    if (file.ChunkInfos == null)
                    {
                        Debug.WriteLine($"开始校验文件:{filePath}");
                        var checkResult = await VaildateFullFile(file, filePath);
                        if (checkResult)
                        {
                            Debug.WriteLine($"{filePath}需要全量更新");
                            HttpClientService.BuildClient();
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
                            Debug.WriteLine($"{filePath}更新结束");
                            await FinalValidation(file, filePath);
                        }
                    }
                    else
                    {
                        var fileName = System.IO.Path.GetFileName(filePath);
                        Debug.WriteLine($"共计{file.ChunkInfos.Count}个分片待检");
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
                                    Debug.WriteLine(
                                        $"正在更新最后分片{fileName}需要更新，开始下载"
                                    );
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
                                    Debug.WriteLine($"{fileName}需要更新，开始下载分片{i}");
                                    await DownloadFileByChunks(
                                        file,
                                        filePath,
                                        file.ChunkInfos[i],
                                        false
                                    );
                                }

                                Debug.WriteLine($"分片{i}更新完毕");
                            }
                        }
                    }
                    Debug.WriteLine($"文件更新结束");
                    await FinalValidation(file, filePath);
                }
                else
                {
                    HttpClientService.BuildClient();
                    Debug.WriteLine($"文件不存在，开始全量更新");
                    await DownloadFileByChunks(
                        file,
                        filePath,
                        new IndexChunkInfo()
                        {
                            Start = 0,
                            End = file.Size - 1,
                            Md5 = file.Md5,
                        }
                    );
                    Debug.WriteLine($"文件更新完毕");
                    await FinalValidation(file, filePath);
                }
            }
        }
        catch (IOException ex)
        {
            Debug.WriteLine(ex.Message);
        }
        finally
        {
            _globalSpeedWatch.Stop();
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
                81920,
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
                const int MaxBufferSize = 1 * 1024 * 1024; // 1MB
                var memoryPool = ArrayPool<byte>.Shared;
                long offset = file.Start;
                long remaining = file.End - file.Start + 1;
                bool isValid = true;
                using (var md5 = MD5.Create())
                {
                    while (remaining > 0 && isValid)
                    {
                        var buffer = memoryPool.Rent(MaxBufferSize);
                        try
                        {
                            fs.Seek(offset, SeekOrigin.Begin);
                            int bytesRead = await fs.ReadAsync(buffer, 0, MaxBufferSize);
                            if (bytesRead == 0)
                            {
                                Debug.WriteLine("提前到达文件末尾");
                                isValid = false;
                                break;
                            }
                            md5.TransformBlock(buffer, 0, bytesRead, null, 0);
                            offset += bytesRead;
                            remaining -= bytesRead;
                        }
                        catch (IOException ex) { }
                        finally
                        {
                            memoryPool.Return(buffer);
                        }
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
        try
        {
            using (
                var fs = new FileStream(
                    filePath,
                    FileMode.Open,
                    FileAccess.ReadWrite,
                    FileShare.ReadWrite,
                    81920,
                    true
                )
            )
            {
                var fullHash = await ComputeFullHash(fs);
                if (fullHash.ToLower() != file.Md5.ToLower())
                {
                    return true;
                }
                return false;
            }
        }
        catch (IOException ex)
        {
            return false;
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
                    FileShare.None,
                    81920,
                    true
                )
            )
            {
                using var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    _downloadBaseUrl + file.Dest
                );
                request.Headers.Range = new RangeHeaderValue(chunk.Start, chunk.End);
                using var response = await HttpClientService.GameDownloadClient.SendAsync(
                    request,
                    _downloadCTS.Token
                );
                var stream = await response.Content.ReadAsStreamAsync(_downloadCTS.Token);
                Debug.WriteLine("请求成功，开始写入分片");
                try
                {
                    // 预校验分片范围
                    if (chunk.Start < 0 || chunk.End < chunk.Start)
                    {
                        throw new ArgumentException($"分片范围无效: {chunk.Start}-{chunk.End}");
                    }

                    long totalWritten = 0;
                    long chunkTotalSize = chunk.End - chunk.Start + 1;
                    const int MaxBufferSize = 4096;
                    var memoryPool = ArrayPool<byte>.Shared;
                    fileStream.Seek(chunk.Start, SeekOrigin.Begin);
                    while (totalWritten < chunkTotalSize)
                    {
                        int bytesToRead = (int)
                            Math.Min(MaxBufferSize, chunkTotalSize - totalWritten);
                        byte[] buffer = memoryPool.Rent(bytesToRead);
                        try
                        {
                            Memory<byte> bufferMemory = buffer.AsMemory(0, bytesToRead);
                            var bytesRead = await stream.ReadAsync(bufferMemory);
                            if (bytesRead == 0)
                            {
                                throw new IOException(
                                    $"数据流提前结束，预期 {chunkTotalSize} 字节，已写入 {totalWritten} 字节"
                                );
                            }
                            await fileStream.WriteAsync(buffer, 0, bytesRead); // 或使用同步写入 fileStream.Write(buffer.AsSpan(0, bytesRead));
                            totalWritten += bytesRead;
                        }
                        finally
                        {
                            memoryPool.Return(buffer);
                        }
                    }
                    if (totalWritten != chunkTotalSize)
                    {
                        throw new IOException($"分片写入不完整: {totalWritten}/{chunkTotalSize}");
                    }
                    else
                    {
                        if (isLast)
                        {
                            fileStream.SetLength(allSize);
                        }
                    }
                }
                catch (IOException ex)
                {
                    Debug.WriteLine($"IO异常: {ex.Message}");
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

    private async Task DownloadFileByFull(IndexResource file, string filePath, IndexChunkInfo chunk)
    {
        try
        {
            using (
                var fileStream = new FileStream(
                    filePath,
                    FileMode.OpenOrCreate,
                    FileAccess.ReadWrite,
                    FileShare.None,
                    81920,
                    true
                )
            )
            {
                using var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    _downloadBaseUrl + file.Dest
                );
                request.Headers.Range = new RangeHeaderValue(chunk.Start, chunk.End);
                using var response = await HttpClientService.GameDownloadClient.SendAsync(
                    request,
                    _downloadCTS.Token
                );
                var stream = await response.Content.ReadAsStreamAsync(_downloadCTS.Token);
                Debug.WriteLine("请求成功，开始写入分片");
                try
                {
                    // 预校验分片范围
                    if (chunk.Start < 0 || chunk.End < chunk.Start)
                    {
                        throw new ArgumentException($"分片范围无效: {chunk.Start}-{chunk.End}");
                    }

                    long totalWritten = 0;
                    long chunkTotalSize = chunk.End - chunk.Start + 1;
                    const int MaxBufferSize = 4096;
                    var memoryPool = ArrayPool<byte>.Shared;
                    fileStream.Seek(chunk.Start, SeekOrigin.Begin);
                    while (totalWritten < chunkTotalSize)
                    {
                        int bytesToRead = (int)
                            Math.Min(MaxBufferSize, chunkTotalSize - totalWritten);
                        byte[] buffer = memoryPool.Rent(bytesToRead);
                        try
                        {
                            Memory<byte> bufferMemory = buffer.AsMemory(0, bytesToRead);
                            var bytesRead = await stream.ReadAsync(bufferMemory);
                            if (bytesRead == 0)
                            {
                                throw new IOException(
                                    $"数据流提前结束，预期 {chunkTotalSize} 字节，已写入 {totalWritten} 字节"
                                );
                            }
                            await fileStream.WriteAsync(buffer, 0, bytesRead); // 或使用同步写入 fileStream.Write(buffer.AsSpan(0, bytesRead));
                            totalWritten += bytesRead;
                        }
                        finally
                        {
                            memoryPool.Return(buffer);
                        }
                    }
                    if (totalWritten != chunkTotalSize)
                    {
                        throw new IOException($"分片写入不完整: {totalWritten}/{chunkTotalSize}");
                    }
                    else
                    {
                        fileStream.SetLength(file.Size);
                    }
                }
                catch (IOException ex)
                {
                    Debug.WriteLine($"IO异常: {ex.Message}");
                    throw;
                }
                stream.Close();
                await stream.DisposeAsync();
            }
        }
        catch (IOException ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    private async Task WriteChunkWithLock(
        FileStream fileStream,
        IndexChunkInfo chunk,
        Stream dataStream,
        CancellationToken ct
    ) { }

    #endregion

    #region 辅助方法
    private List<IndexChunkInfo> MergeAdjacentChunks(List<IndexChunkInfo> chunks)
    {
        if (chunks.Count < 2)
            return chunks;

        var sorted = chunks.OrderBy(c => c.Start).ToList();
        var merged = new List<IndexChunkInfo> { sorted[0] };

        for (int i = 1; i < sorted.Count; i++)
        {
            var last = merged.Last();
            if (sorted[i].Start <= last.End + 1)
            {
                merged[merged.Count - 1] = new IndexChunkInfo
                {
                    Start = last.Start,
                    End = Math.Max(last.End, sorted[i].End),
                };
            }
            else
            {
                merged.Add(sorted[i]);
            }
        }
        return merged;
    }

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
                    81920,
                    true
                )
            )
            {
                var fullHash = await ComputeFullHash(fs);
                if (fullHash.ToLower() != file.Md5.ToLower())
                {
                    Debug.WriteLine($"文件{filePath}检查出错！");
                }
                else
                {
                    Debug.WriteLine($"文件{filePath}检查成功，算法正确");
                    await fs.FlushAsync();
                }
            }
        }
        catch (Exception ex) { }
    }

    private void UpdateFileProgress(
        long currentFile,
        long totalFiles,
        long fileSize,
        long totalSize
    )
    {
        // 严格保持参数不变，fileSize参数按原始逻辑使用
        _totalDownloadedBytes += fileSize; // 新增：实际使用fileSize参数
        UpdateDownloadProgress(
            GameContextActionType.Work,
            currentFile: currentFile + 1,
            maxFile: totalFiles,
            currentSize: _totalDownloadedBytes, // 使用类字段
            maxSize: totalSize,
            speed: 0,
            speedString: "0 B/s"
        );
    }

    private double CalculateAverageSpeed(double currentSpeedInMBps)
    {
        var currentSpeedInBytesPerSecond = currentSpeedInMBps * 1024 * 1024;
        _speedHistory.Enqueue(currentSpeedInBytesPerSecond);
        if (_speedHistory.Count > SpeedHistoryWindowSize)
        {
            _speedHistory.Dequeue();
        }
        return _speedHistory.Average();
    }

    private void UpdateDownloadProgress(
        GameContextActionType type,
        long currentFile,
        long maxFile,
        long currentSize,
        long maxSize,
        double speed = 0.0,
        string speedString = "0MB/s"
    )
    {
        try
        {
            var progress = Math.Round((double)currentSize / maxSize * 100, 2);
            var averageSpeed = CalculateAverageSpeed(speed);
            // 计算剩余时间
            string remainingTimeString = "N/A";
            var speedValue = speed;
            if (averageSpeed > 0)
            {
                var remainingBytes = maxSize - currentSize;
                var remainingTime = TimeSpan.FromSeconds(remainingBytes / speedValue);
                remainingTimeString = $"{remainingTime:hh\\:mm\\:ss}";
            }

            this.gameContextOutputDelegate?.Invoke(
                this,
                new GameContextOutputArgs()
                {
                    Type = type,
                    CurrentFile = (int)currentFile,
                    MaxFile = maxFile,
                    CurrentSize = currentSize,
                    MaxSize = maxSize,
                    Speed = speed,
                    SpeedString = speedString,
                    Progress = progress,
                    RemainingTime = remainingTimeString,
                }
            );
        }
        catch (IOException ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    private void UpdateProgressMetrics(long bytesRead, Stopwatch speedWatch)
    {
        _totalDownloadedBytes += bytesRead;

        if (speedWatch.Elapsed.TotalSeconds > 0)
        {
            var currentSpeed = bytesRead / speedWatch.Elapsed.TotalSeconds;
            _speedHistory.Enqueue(currentSpeed);

            if (_speedHistory.Count > SpeedHistoryWindowSize)
                _speedHistory.Dequeue();
        }

        var avgSpeed = _speedHistory.Any() ? _speedHistory.Average() : 0;
        var (speedValue, unit) = FormatSpeed(avgSpeed);

        gameContextOutputDelegate?.Invoke(
            this,
            new GameContextOutputArgs
            {
                Speed = avgSpeed,
                SpeedString = $"{speedValue:0.00} {unit}/s",
                CurrentSize = _totalDownloadedBytes,
            }
        );
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

    private (double value, string unit) FormatSpeed(double bytesPerSecond)
    {
        string[] units = { "B", "KB", "MB", "GB" };
        int unitIndex = 0;
        while (bytesPerSecond >= 1024 && unitIndex < units.Length - 1)
        {
            bytesPerSecond /= 1024;
            unitIndex++;
        }
        return (bytesPerSecond, units[unitIndex]);
    }

    private void InitializeProgress(IndexGameResource resource)
    {
        gameContextOutputDelegate?.Invoke(
            this,
            new GameContextOutputArgs
            {
                CurrentFile = 0,
                MaxFile = resource.Resource.Count,
                CurrentSize = 0,
                MaxSize = resource.Resource.Sum(x => x.Size),
                Progress = 0,
                Speed = 0,
                SpeedString = "正在准备",
                Type = GameContextActionType.Work,
                RemainingTime = "00:00:00",
            }
        );
    }
    #endregion

    #region 限速器实现
    #endregion
}
