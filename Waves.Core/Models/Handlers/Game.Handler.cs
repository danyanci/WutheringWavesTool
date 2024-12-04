using Waves.Core.Models;

namespace Waves.Core.GameContext;

public delegate void GameVerifyDelegate();

public delegate void GameDownloadDelegate();

public delegate void GameContextOutputDelegate(object sender, GameContextOutputArgs args);

partial class GameContextBase
{
    private GameContextOutputDelegate? gameContextOutputDelegate;

    public event GameContextOutputDelegate GameContextOutput
    {
        add => gameContextOutputDelegate += value;
        remove => gameContextOutputDelegate -= value;
    }
}
