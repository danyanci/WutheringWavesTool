using Waves.Core.Models.Enums;

namespace Waves.Core.Models;

public class GameContextOutputArgs
{
    public GameContextActionType Type { get; set; }

    public double Progress { get; internal set; }

    public int MaxFile { get; internal set; }

    public int CurrentFile { get; internal set; }

    public double MaxSize { get; internal set; }

    public double CurrentSize { get; internal set; }

    public string SpeedString { get; internal set; }

    public double Speed { get; set; }
    public string RemainingTime { get; internal set; }

    public string ErrorMessage { get; internal set; }
}
