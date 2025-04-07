using System.Text.Json;
using Waves.Api.Models;
using Waves.Core.Models.Downloader;

namespace Waves.Core.GameContext;

partial class GameContextBase
{
    public async Task<GameLauncherSource?> GetGameLauncherSourceAsync(
        CancellationToken token = default
    )
    {
        var result = await HttpClientService.GameDownloadClient.GetAsync(
            this.Config.Launcher_Source
        );
        var jsonStr = await result.Content.ReadAsStringAsync();
        var launcherIndex = JsonSerializer.Deserialize<GameLauncherSource>(
            jsonStr,
            GameLauncherSourceContext.Default.GameLauncherSource
        );
        return launcherIndex;
    }

    public async Task<IndexGameResource> GetGameResourceAsync(
        ResourceDefault ResourceDefault,
        CancellationToken token = default
    )
    {
        var resourceIndexUrl =
            ResourceDefault.CdnList.Where(x => x.P != 0).OrderBy(x => x.P).First().Url
            + ResourceDefault.Config.IndexFile;
        var result = await HttpClientService.HttpClient.GetAsync(resourceIndexUrl, token);
        var jsonStr = await result.Content.ReadAsStringAsync();
        var launcherIndex = JsonSerializer.Deserialize<IndexGameResource>(
            jsonStr,
            IndexGameResourceContext.Default.IndexGameResource
        );
        return launcherIndex;
    }

    public async Task<PatchIndexGameResource?> GetPatchGameResourceAsync(
        string url,
        CancellationToken token = default
    )
    {
        try
        {
            var result = await HttpClientService.HttpClient.GetAsync(url, token);
            result.EnsureSuccessStatusCode();
            var jsonStr = await result.Content.ReadAsStringAsync();
            var pathIndexSource = JsonSerializer.Deserialize<PatchIndexGameResource>(
                jsonStr,
                PathIndexGameResourceContext.Default.PatchIndexGameResource
            );
            return pathIndexSource;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<GameLauncherStarter?> GetLauncherStarterAsync(
        CancellationToken token = default
    )
    {
        try
        {
            var result = await HttpClientService.HttpClient.GetAsync(
                this.Config.Starter_Source,
                token
            );
            result.EnsureSuccessStatusCode();
            var jsonStr = await result.Content.ReadAsStringAsync();
            var pathIndexSource = JsonSerializer.Deserialize<GameLauncherStarter>(
                jsonStr,
                GameLauncherStarterContext.Default.GameLauncherStarter
            );
            return pathIndexSource;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
