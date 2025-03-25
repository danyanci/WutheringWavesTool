using System.Text.Json;
using WutheringWaves.Core.Common;
using WutheringWaves.Core.Models;

namespace WutheringWaves.Core.GameContext;

partial class GameContextBase
{
    public async Task<GameLauncherModel?> GetGameLauncherAsync()
    {
        try
        {
            var respose = await this.HttpClientService.HttpClient.GetAsync(
                this.Config.Launcher_Source
            );
            respose.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<GameLauncherModel>(
                await respose.Content.ReadAsStringAsync(),
                JsonCoreSerializer.Default.GameLauncherModel
            );
        }
        catch (Exception)
        {
            throw;
        }
    }
}
