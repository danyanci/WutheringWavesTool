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
}
