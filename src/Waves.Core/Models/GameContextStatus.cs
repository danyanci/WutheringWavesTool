namespace Waves.Core.Models;

public class GameContextStatus
{
    /// <summary>
    /// 游戏是否已经安装
    /// </summary>
    public bool IsGameExists { get; internal set; }

    /// <summary>
    /// 游戏是否下载完毕
    /// </summary>
    public bool IsGameInstalled { get; internal set; }
}
