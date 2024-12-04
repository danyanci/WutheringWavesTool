using Waves.Core.Models;

namespace Waves.Core.GameContext.Contexts;

public class MainGameContext : GameContextBase
{
    public MainGameContext(GameApiContextConfig config)
        : base(config, nameof(MainGameContext)) { }

    public override bool IsLaunch => true;
}
