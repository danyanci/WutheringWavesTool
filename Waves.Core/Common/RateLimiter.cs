namespace Waves.Core.Common;

public class RateLimiter
{
    private readonly object _locker = new object();
    private long _tokens;
    private readonly long _capacity;
    private readonly long _fillRate;
    private DateTime _lastRefillTime;

    public RateLimiter(long bytesPerSecond)
    {
        _capacity = bytesPerSecond;
        _fillRate = bytesPerSecond;
        _tokens = _capacity;
        _lastRefillTime = DateTime.UtcNow;
    }

    public async Task<int> ConsumeAndReadAsync(
        Stream responseStream,
        FileStream fileStream,
        byte[] buffer,
        CancellationToken token
    )
    {
        int bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length, token);
        await ConsumeAsync(bytesRead);
        await fileStream.WriteAsync(buffer, 0, bytesRead, token);
        return bytesRead;
    }

    private void RefillTokens()
    {
        var now = DateTime.UtcNow;
        var elapsed = (now - _lastRefillTime).TotalSeconds;
        _tokens = (long)Math.Min(_capacity, _tokens + elapsed * _fillRate);
        _lastRefillTime = now;
    }

    public async Task<int> MaybeLimitAndReadAsync(
        Stream responseStream,
        FileStream fileStream,
        byte[] buffer,
        CancellationToken token,
        bool isLimitSpeed,
        RateLimiter rateLimiter
    )
    {
        int bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length, token);
        if (isLimitSpeed)
        {
            await rateLimiter.ConsumeAsync(bytesRead);
        }
        await fileStream.WriteAsync(buffer, 0, bytesRead, token);
        return bytesRead;
    }

    public async Task ConsumeAsync(long bytesToConsume)
    {
        lock (_locker)
        {
            RefillTokens();

            while (_tokens < bytesToConsume)
            {
                var neededTokens = bytesToConsume - _tokens;
                var waitTime = neededTokens / _fillRate;
                Monitor.Wait(_locker, TimeSpan.FromSeconds(waitTime));
                RefillTokens();
            }

            _tokens -= bytesToConsume;
        }

        var delay = bytesToConsume / _fillRate;
        await Task.Delay(TimeSpan.FromSeconds(delay));
    }
}
