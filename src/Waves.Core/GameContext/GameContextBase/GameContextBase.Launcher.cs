using System.Text.Json;
using Waves.Api.Models;

namespace Waves.Core.GameContext;

partial class GameContextBase
{
    public async Task<LauncherHeader?> GetLauncherHeaderAsync(CancellationToken token = default)
    {
        var url = this.Config.LauncherHeader_Source;
        var result = await this.HttpClientService.HttpClient.GetAsync(url);
        result.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize(
            await result.Content.ReadAsStringAsync(),
            LauncherHeaderContext.Default.LauncherHeader
        );
    }

    public async Task<WavesIndex> GetGameIndexAsync(CancellationToken token = default)
    {
        var result = await HttpClientService.GameDownloadClient.GetAsync(this.Config.Index_Source);
        var launcherIndex = JsonSerializer.Deserialize<WavesIndex>(
            await result.Content.ReadAsStringAsync(),
            WavesIndexContext.Default.WavesIndex
        );
        return launcherIndex;
    }

    public async Task<GameResource> GetGameResourceAsync(
        string resourceUrl,
        CancellationToken token = default
    )
    {
        var result = await HttpClientService.GameDownloadClient.GetAsync(resourceUrl);
        var launcherIndex = JsonSerializer.Deserialize<GameResource>(
            await result.Content.ReadAsStringAsync(),
            GameResourceContext.Default.GameResource
        );
        return launcherIndex;
    }

    public async Task<GameLauncherSource> GetGameLauncherSourceAsync(
        CancellationToken token = default
    )
    {
        var result = await HttpClientService.GameDownloadClient.GetAsync(
            this.Config.Launcher_Source
        );
        var launcherIndex = JsonSerializer.Deserialize<GameLauncherSource>(
            await result.Content.ReadAsStringAsync(),
            GameLauncherSourceContext.Default.GameLauncherSource
        );
        return launcherIndex;
    }

    public async Task<GameLauncherStarter?> GetGameLauncherStarterAsync(
        GameLauncherSource launcheSource,
        bool ishans,
        CancellationToken token = default
    )
    {
        var cdn = launcheSource.Default.CdnList.OrderByDescending(x => x.P).LastOrDefault();
        try
        {
            if (cdn != null)
            {
                string sourceUrl = "";
                if (ishans)
                    sourceUrl = cdn.Url + launcheSource.NavigationBarLanguage.ZhHans;
                else
                    sourceUrl = cdn.Url + launcheSource.NavigationBarLanguage.ZhHant;
                var httpResult = await HttpClientService.GameDownloadClient.GetAsync(sourceUrl);
                httpResult.EnsureSuccessStatusCode();
                var str = await httpResult.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize(
                    str,
                    GameLauncherStarterContext.Default.GameLauncherStarter
                );
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}
