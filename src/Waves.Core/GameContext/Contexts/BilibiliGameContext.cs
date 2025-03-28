using Waves.Core.GameContext;
using Waves.Core.Models;

namespace Waves.Core.GameContext.Contexts;

public class BilibiliGameContext : GameContextBase
{
    internal BilibiliGameContext(GameApiContextConfig config)
        : base(config, nameof(BilibiliGameContext)) { }

    public override Type ContextType => typeof(BilibiliGameContext);
}
