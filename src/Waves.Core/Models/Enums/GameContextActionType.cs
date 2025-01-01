namespace Waves.Core.Models.Enums;

public enum GameContextActionType
{
    Verify,
    Download,
    Clear,
    Error,
    None,
    ProdDownload,
}

/// <summary>
/// 游戏进行下载动作时的来源
/// </summary>
public enum GameDownloadActionSource
{
    /// <summary>
    /// 修复本地游戏
    /// </summary>
    Verify,

    /// <summary>
    /// 下载游戏
    /// </summary>
    Download,

    /// <summary>
    /// 更新游戏
    /// </summary>
    Update,

    /// <summary>
    /// 预下载游戏
    /// </summary>
    ProdDownload,
}
