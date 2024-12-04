using Waves.Core.Models.Enums;

namespace Waves.Core.Models;

public class GameContextOutputArgs
{
    public GameContextActionType Type { get; set; }

    public double Progress { get; set; }

    public int MaxFile { get; set; }

    public int CurrentFile { get; set; }

    public int MaxSize { get; set; }

    public int CurrentSize { get; set; }

    public string SpeedString { get; set; }

    public int Speed { get; set; }
}
