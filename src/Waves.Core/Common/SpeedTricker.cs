namespace Waves.Core.Common;

public class SpeedTracker
{
    private long _totalBytes;
    private DateTime _startTime = DateTime.Now;
    private readonly SemaphoreSlim _semaphore = new(1, 1); // 异步锁

    public async Task AccumulateAsync(long bytes, DateTime timestamp)
    {
        await _semaphore.WaitAsync(); // 异步获取锁
        try
        {
            _totalBytes += bytes;
            if (_startTime > timestamp)
                _startTime = timestamp;
        }
        finally
        {
            _semaphore.Release(); // 释放锁
        }
    }

    public async Task<double> GetSpeedAsync(DateTime lastUpdate)
    {
        await _semaphore.WaitAsync(); // 异步获取锁
        try
        {
            var elapsed = (DateTime.Now - _startTime).TotalSeconds;
            if (elapsed <= 0) // 避免除以零
                return 0;

            var speed = _totalBytes / elapsed;
            _totalBytes = 0; // 重置总字节数
            _startTime = DateTime.Now; // 重置开始时间
            lastUpdate = _startTime;
            return speed;
        }
        finally
        {
            _semaphore.Release(); // 释放锁
        }
    }
}
