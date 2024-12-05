using System.Text;
using System.Text.Json;
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
        if (!File.Exists(BaseGameFolder + "\\downloadCahce.json"))
        {
            this.Cache = new DownloadCache();
            return Cache;
        }
        var json = await File.ReadAllTextAsync(BaseGameFolder + "\\downloadCahce.json");
        try
        {
            this.Cache = JsonSerializer.Deserialize(
                json,
                DownloadCacheJsonContext.Default.DownloadCache
            )!;
        }
        catch (Exception)
        {
            Cache = new DownloadCache();
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
            if (!File.Exists(file))
            {
                using (var fs = File.CreateText(file))
                {
                    await fs.WriteAsync(json);
                }
                return true;
            }
            else
            {
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Write))
                {
                    await fs.WriteAsync(Encoding.UTF8.GetBytes(json));
                }
                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }
}
