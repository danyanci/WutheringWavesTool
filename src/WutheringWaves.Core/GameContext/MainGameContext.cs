using WutheringWaves.Core.Models;

namespace WutheringWaves.Core.GameContext
{
    public class MainGameContext : GameContextBase
    {
        public MainGameContext(GameApiContextConfig config, string contextName)
            : base(config, contextName) { }

        public bool IsLimitSpeed { get; internal set; }
    }
}
