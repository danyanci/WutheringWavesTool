namespace WutheringWaves.Core.Models;

public class GameCoreStatus
{
    /// <summary>
    /// 是否在下载
    /// </summary>
    public bool IsDownload { get; internal set; }

    /// <summary>
    /// 正在校验游戏
    /// </summary>
    public bool IsVerifyGame { get; internal set; }

    /// <summary>
    /// 游戏是否可以启动
    /// </summary>
    public bool GameExists { get; internal set; }

    /// <summary>
    /// 游戏是否是正在下载状态
    /// </summary>
    public bool GameIsStartDownload { get; internal set; }
}
