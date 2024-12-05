using Waves.Core.Models;

namespace Waves.Core.Contracts;

public interface IGameContextDownloadCache
{
    public string BaseGameFolder { get; }

    public Task<bool> WriteCacheAsync(DownloadCache cache);

    public Task<DownloadCache> ReadCacheAsync();
}
