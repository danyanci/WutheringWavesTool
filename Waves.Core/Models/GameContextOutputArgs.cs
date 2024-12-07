using Waves.Core.Models.Enums;

namespace Waves.Core.Models;

public class GameContextOutputArgs
{
    public GameContextActionType Type { get; set; }

    public double Progress { get; set; }

    public int MaxFile { get; set; }

    public int CurrentFile { get; set; }

    public double MaxSize { get; set; }

    public double CurrentSize { get; set; }

    public string SpeedString { get; set; }

    public double Speed { get; set; }
    public string RemainingTime { get; internal set; }
}
