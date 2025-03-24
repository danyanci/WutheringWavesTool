using WutheringWaves.Core.Models;

namespace WutheringWaves.Core.Contracts;

public interface IGameContext
{
    public IHttpClientService HttpClientService { get; }

    public Task<GameLauncherModel> GetGameLauncherAsync();
}
