using WutheringWaves.Core.Contracts;
using WutheringWaves.Core.Models;

namespace WutheringWaves.Core.GameContext;

public partial class GameContextBase : IGameContext
{
    public GameContextBase(GameApiContextConfig config, string contextName)
    {
        Config = config;
        ContextName = contextName;
    }

    public GameApiContextConfig Config { get; }
    public string ContextName { get; }
    public IHttpClientService HttpClientService { get; internal set; }

    public Task<GameLauncherModel> GetGameLauncherAsync()
    {
        throw new NotImplementedException();
    }
}
