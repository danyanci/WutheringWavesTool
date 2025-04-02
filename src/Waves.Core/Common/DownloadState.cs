using System.Collections.Concurrent;
using Waves.Core.Models.Downloader;

namespace Waves.Core.Common;

/// <summary>
/// 下载状态机
/// </summary>
public sealed class DownloadState
{
    private readonly SemaphoreSlim _pauseLock = new(1, 1);
    private readonly SemaphoreSlim _speedLimiterSemaphore = new(1, 1);
    private long _currentBytes;

    public SpeedLimiter SpeedLimiter { get; set; }

    public DownloadState(IndexGameResource resources)
    {
        Resources = resources;
        SpeedLimiter = new SpeedLimiter();
    }

    public async Task SetSpeedLimitAsync(long byteSecond)
    {
        await _speedLimiterSemaphore.WaitAsync();
        try
        {
            var newLimiter = new SpeedLimiter();
            await newLimiter.SetBytesPerSecondAsync(byteSecond);
            SpeedLimiter = newLimiter;
        }
        finally
        {
            _speedLimiterSemaphore.Release();
        }
    }

    public IndexGameResource Resources { get; }
    public long TotalSize { get; set; }
    public long CurrentSize { get; set; }
    public bool IsPaused { get; private set; }
    public bool IsActive { get; set; }

    /// <summary>
    /// 取消信号
    /// </summary>
    public CancellationToken CancelToken { get; set; }

    public PauseToken PauseToken => new(_pauseLock);

    /// <summary>
    /// 异步暂停（基于信号量）
    /// </summary>
    /// <returns></returns>
    public async Task<bool> PauseAsync()
    {
        await _pauseLock.WaitAsync();
        try
        {
            IsPaused = true;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            _pauseLock.Release();
        }
    }

    /// <summary>
    /// 异步恢复（基于信号量）
    /// </summary>
    /// <returns></returns>
    public async Task<bool> ResumeAsync()
    {
        await _pauseLock.WaitAsync();
        try
        {
            IsPaused = false;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            _pauseLock.Release();
        }
    }
}

public readonly struct PauseToken
{
    private readonly SemaphoreSlim _semaphore;

    public PauseToken(SemaphoreSlim semaphore) => _semaphore = semaphore;

    /// <summary>
    /// 异步阻塞等待信号
    /// </summary>
    /// <param name="isPaused"></param>
    /// <returns></returns>
    public async Task WaitIfPausedAsync(Func<bool> isPaused)
    {
        while (isPaused())
        {
            await _semaphore.WaitAsync();
            try
            {
                if (isPaused())
                    await Task.Delay(500);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
