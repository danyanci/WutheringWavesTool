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

        // 初始化限速器（默认1MB/s）
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
        catch (Exception ex)
        {
            // 处理异常...
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
                var (invalidChunks, needFullDownload) = await ValidateFileChunks(file, filePath);

                if (invalidChunks.Count > 0)
                {
                    await DownloadFileByChunks(
                        file,
                        filePath,
                        invalidChunks,
                        totalFiles,
                        totalSize
                    );
                }
                else
                {
                    UpdateFileProgress(
                        currentFile: 0,
                        totalFiles: totalFiles,
                        fileSize: file.Size,
                        totalSize: totalSize
                    );
                }
            }
        }
        finally
        {
            _globalSpeedWatch.Stop();
        }
    }
    #endregion

    #region 校验逻辑
    private async Task<(
        List<IndexChunkInfo> invalidChunks,
        bool needFullDownload
    )> ValidateFileChunks(IndexResource file, string filePath)
    {
        var invalidChunks = new List<IndexChunkInfo>();

        if (!File.Exists(filePath))
        {
            return (
                file.ChunkInfos
                    ?? new List<IndexChunkInfo>
                    {
                        new() { Start = 0, End = file.Size - 1 },
                    },
                true
            );
        }

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
            if (fs.Length != file.Size)
            {
                var result = (
                    file.ChunkInfos
                        ?? new List<IndexChunkInfo>
                        {
                            new() { Start = 0, End = file.Size - 1 },
                        },
                    true
                );
                return result;
            }

            if (file.ChunkInfos?.Count > 0)
            {
                foreach (var chunk in file.ChunkInfos)
                {
                    var buffer = new byte[(chunk.End - chunk.Start + 1)];
                    try
                    {
                        fs.Seek(chunk.Start, SeekOrigin.Begin);
                        var bytesRead = await fs.ReadAsync(
                            buffer,
                            0,
                            (int)(chunk.End - chunk.Start + 1)
                        );
                        using (MD5 md5 = MD5.Create())
                        {
                            var hash = BitConverter
                                .ToString(md5.ComputeHash(buffer))
                                .Replace("-", "")
                                .ToLower();
                            var result = hash != chunk.Md5.ToLower() ? chunk : null;
                            if (result != null)
                                invalidChunks.Add(result);
                        }
                    }
                    finally { }
                }
            }
            else
            {
                var fullHash = await ComputeFullHash(fs);
                if (fullHash.ToLower() != file.Md5.ToLower())
                {
                    invalidChunks.Add(new IndexChunkInfo { Start = 0, End = file.Size - 1 });
                }
            }
            return (MergeAdjacentChunks(invalidChunks), invalidChunks.Count > 0);
        }
    }
    #endregion

    #region 下载逻辑
    private async Task DownloadFileByChunks(
        IndexResource file,
        string filePath,
        List<IndexChunkInfo> chunks,
        int totalFiles,
        long totalSize
    )
    {
        using (
            var fileStream = new FileStream(
                filePath,
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite,
                FileShare.Read,
                81920,
                true
            )
        )
        {
            for (int i = 0; i < chunks.Count; i++)
            {
                using var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    _downloadBaseUrl + file.Dest
                );
                request.Headers.Range = new RangeHeaderValue(chunks[i].Start, chunks[i].End);

                using var response = await HttpClientService.GameDownloadClient.SendAsync(
                    request,
                    _downloadCTS.Token
                );
                using var stream = await response.Content.ReadAsStreamAsync(_downloadCTS.Token);

                await WriteChunkWithLock(fileStream, chunks[i], stream, _downloadCTS.Token);
                await FinalValidation(file, filePath);
            }
        }
    }

    private async Task WriteChunkWithLock(
        FileStream fileStream,
        IndexChunkInfo chunk,
        Stream dataStream,
        CancellationToken ct
    )
    {
        var buffer = new byte[81920];
        try
        {
            long totalWritten = 0;
            var speedWatch = Stopwatch.StartNew();

            while (totalWritten < chunk.End - chunk.Start + 1)
            {
                var bytesToRead = (int)
                    Math.Min(buffer.Length, chunk.End - chunk.Start + 1 - totalWritten);
                var bytesRead = await dataStream.ReadAsync(buffer, 0, bytesToRead, ct);

                await _speedLimiter.Limit(bytesRead);

                lock (fileStream)
                {
                    fileStream.Seek(chunk.Start + totalWritten, SeekOrigin.Begin);
                    fileStream.Write(buffer, 0, bytesRead);
                }

                totalWritten += bytesRead;
                UpdateProgressMetrics(bytesRead, speedWatch);
            }
        }
        finally
        {
            await fileStream.FlushAsync();
        }
    }
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
        using var fs = File.OpenRead(filePath);
        if (file.ChunkInfos?.Count > 0)
        {
            var invalidChunks = await ValidateFileChunks(file, filePath);
            if (invalidChunks.invalidChunks.Count > 0)
                throw new Exception(
                    $"最终校验失败，剩余错误分块：{invalidChunks.invalidChunks.Count}"
                );
        }
        else
        {
            var hash = await ComputeFullHash(fs);
            if (hash.ToLower() != file.Md5.ToLower())
                throw new Exception("文件整体校验失败");
        }
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
        catch (Exception ex) { }
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
