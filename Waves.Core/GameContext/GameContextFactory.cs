using Waves.Core.GameContext.Contexts;
using Waves.Core.Models;

namespace Waves.Core.GameContext;

public static class GameContextFactory
{
    public static string GameBassPath { get; set; }

    internal static BilibiliGameContext GetBilibiliGameContext() =>
        new BilibiliGameContext(GameApiContextConfig.BiliBili)
        {
            GamerConfigPath = GameContextFactory.GameBassPath + "\\BiliBiliConfig",
        };

    internal static GlobalGameContext GetGlobalGameContext() =>
        new GlobalGameContext(GameApiContextConfig.Global)
        {
            GamerConfigPath = GameContextFactory.GameBassPath + "\\GlobalConfig",
        };

    internal static MainGameContext GetMainGameContext() =>
        new MainGameContext(GameApiContextConfig.Main)
        {
            GamerConfigPath = GameContextFactory.GameBassPath + "\\MainConfig",
        };
}
