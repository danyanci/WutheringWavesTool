using System.Text.Json;
using Waves.Api.Models;

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
        var launcherIndex = JsonSerializer.Deserialize<GameLauncherSource>(
            await result.Content.ReadAsStringAsync(),
            GameLauncherSourceContext.Default.GameLauncherSource
        );
        return launcherIndex;
    }
}
