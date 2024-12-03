using Waves.Core.Contracts;

namespace Waves.Core.GameContext;

public interface IGameContext
{
    public IHttpClientService HttpClientService { get; set; }
    public void Init();
    public string ContextName { get; }
}
