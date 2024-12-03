using Waves.Core.GameContext.Contexts;
using Waves.Core.Models;

namespace Waves.Core.GameContext;

public static class GameContextFactory
{
    public static BilibiliGameContext GetBilibiliGameContext() =>
        new BilibiliGameContext(GameContextConfig.BiliBili);

    public static GlobalGameContext GetGlobalGameContext() =>
        new GlobalGameContext(GameContextConfig.Global);

    public static MainGameContext GetMainGameContext() =>
        new MainGameContext(GameContextConfig.Main);
}
