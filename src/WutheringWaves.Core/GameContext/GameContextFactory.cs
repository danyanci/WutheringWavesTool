using WutheringWaves.Core.Models;

namespace WutheringWaves.Core.GameContext;

public static class GameContextFactory
{
    public static string GameBassPath { get; set; }

    internal static MainGameContext GetMainGameContext()
    {
        return new MainGameContext(GameApiContextConfig.Main, nameof(MainGameContext))
        {
            GamerConfigPath = GameBassPath + "\\MainConfig",
            IsLimitSpeed = false,
        };
    }
}
