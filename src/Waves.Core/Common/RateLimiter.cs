namespace Waves.Core.Common;

public class SpeedLimiter
{
    private readonly int _maxBytesPerSecond;
    private int _bytesRemaining;
    private DateTime _lastUpdate;

    public SpeedLimiter(int bytesPerSecond)
    {
        _maxBytesPerSecond = bytesPerSecond;
        _bytesRemaining = bytesPerSecond;
        _lastUpdate = DateTime.Now;
    }

    public async Task Limit(int bytesTransferred)
    {
        var now = DateTime.Now;
        var elapsed = (now - _lastUpdate).TotalSeconds;
        _bytesRemaining += (int)(_maxBytesPerSecond * elapsed);
        _bytesRemaining = Math.Min(_bytesRemaining, _maxBytesPerSecond);

        _bytesRemaining -= bytesTransferred;
        if (_bytesRemaining < 0)
        {
            var deficit = -_bytesRemaining;
            var waitTime = deficit / (double)_maxBytesPerSecond;
            await Task.Delay(TimeSpan.FromSeconds(waitTime));

            // 重置计数器
            _bytesRemaining += (int)(_maxBytesPerSecond * waitTime);
            _lastUpdate = DateTime.Now;
        }
        else
        {
            _lastUpdate = now;
        }
    }
}
