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
    public bool IsSelectDownloadFolder { get; internal set; }

    /// <summary>
    /// 是否在下载中
    /// </summary>
    public bool IsDownload { get; internal set; }

    /// <summary>
    /// 是否在校验中
    /// </summary>
    public bool IsVerify { get; internal set; }
    public bool IsClear { get; internal set; }
    public bool IsDownloadComplete { get; internal set; }
    public double DownloadProcess { get; internal set; }
}
