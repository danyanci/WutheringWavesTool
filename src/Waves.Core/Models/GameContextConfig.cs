namespace Waves.Core.Models;

public class GameContextConfig
{
    /// <summary>
    /// 限速
    /// </summary>
    public int LimitSpeed { get; internal set; }
    public bool IsDx11 { get; internal set; }
}
