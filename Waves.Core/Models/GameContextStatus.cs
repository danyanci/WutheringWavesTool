namespace Waves.Core.Models;

public class GameContextStatus
{
    /// <summary>
    /// 游戏是否已经启动
    /// </summary>
    public bool IsLauncherGame { get; internal set; }

    /// <summary>
    /// 游戏是否已经安装
    /// </summary>
    public bool IsLauncheExists { get; internal set; }
}
