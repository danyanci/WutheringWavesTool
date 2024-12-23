using Waves.Core.Models;

namespace Waves.Core.GameContext.Contexts;

public class MainGameContext : GameContextBase
{
    internal MainGameContext(GameApiContextConfig config)
        : base(config, nameof(MainGameContext)) { }
}
