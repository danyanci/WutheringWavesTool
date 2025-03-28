using Waves.Core.Contracts;
using Waves.Core.GameContext.Contexts;
using Waves.Core.Models;
using Waves.Core.Services;

namespace Waves.Core.GameContext;

public static class GameContextFactory
{
    public static string GameBassPath { get; set; }

    internal static BilibiliGameContext GetBilibiliGameContext() =>
        new BilibiliGameContext(GameApiContextConfig.BiliBili)
        {
            GamerConfigPath = GameContextFactory.GameBassPath + "\\BiliBiliConfig",
            IsLimitSpeed = false,
        };

    internal static GlobalGameContext GetGlobalGameContext() =>
        new GlobalGameContext(GameApiContextConfig.Global)
        {
            GamerConfigPath = GameContextFactory.GameBassPath + "\\GlobalConfig",
            IsLimitSpeed = false,
        };

    internal static MainGameContext GetMainGameContext() =>
        new MainGameContext(GameApiContextConfig.Main)
        {
            GamerConfigPath = GameContextFactory.GameBassPath + "\\MainConfig",
            IsLimitSpeed = false,
        };
}
