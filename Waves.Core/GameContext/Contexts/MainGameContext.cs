using Waves.Core.Models;

namespace Waves.Core.GameContext.Contexts;

public class MainGameContext : GameContextBase
{
    public MainGameContext(GameContextConfig config)
        : base(config, nameof(MainGameContext)) { }
}
