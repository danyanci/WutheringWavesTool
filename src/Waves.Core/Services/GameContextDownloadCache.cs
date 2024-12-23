using System.Text;
using System.Text.Json;
using Waves.Api.Models;
using Waves.Core.Contracts;
using Waves.Core.Models;

namespace Waves.Core.Services;

public class GameContextDownloadCache : IGameContextDownloadCache
{
    public GameContextDownloadCache(string gamefolder)
    {
        this.BaseGameFolder = gamefolder;
    }

    public string BaseGameFolder { get; internal set; }
    public DownloadCache Cache { get; private set; }

    public async Task<DownloadCache> ReadCacheAsync()
    {
        if (!File.Exists(BaseGameFolder + "\\downloadCache.json"))
        {
            this.Cache = new DownloadCache();
            return null;
        }
        var json = await File.ReadAllTextAsync(BaseGameFolder + "\\downloadCache.json");
        try
        {
            this.Cache = JsonSerializer.Deserialize(
                json,
                DownloadCacheJsonContext.Default.DownloadCache
            )!;
        }
        catch (Exception)
        {
            Cache = new();
        }
        return Cache;
    }

    public async Task<bool> WriteCacheAsync(DownloadCache cache)
    {
        try
        {
            var file = BaseGameFolder + "\\downloadCache.json";
            var json = JsonSerializer.Serialize(
                cache,
                DownloadCacheJsonContext.Default.DownloadCache
            );
            if (File.Exists(file))
            {
                File.Delete(file);
            }
            using (var fs = File.CreateText(file))
            {
                await fs.WriteAsync(json);
            }
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
