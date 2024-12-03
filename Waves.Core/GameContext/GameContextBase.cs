using Waves.Core.Contracts;
using Waves.Core.Models;

namespace Waves.Core.GameContext;

public class GameContextBase : IGameContext
{
    public GameContextBase(GameContextConfig config, string contextName)
    {
        Config = config;
        ContextName = contextName;
    }

    public virtual void Init()
    {
        this.HttpClientService.BuildClient(ContextName);
    }

    public IHttpClientService HttpClientService { get; set; }
    public GameContextConfig Config { get; private set; }
    public string ContextName { get; }
}
