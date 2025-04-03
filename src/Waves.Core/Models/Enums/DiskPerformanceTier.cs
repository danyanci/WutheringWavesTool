namespace Waves.Core.Models.Enums;

public enum DiskPerformanceTier
{
    /// <summary>
    /// 接受500MB一秒读写
    /// </summary>
    HighSpeedSSD,

    /// <summary>
    /// 接受200MB到500MB一秒读写
    /// </summary>
    MidRangeSSD,

    /// <summary>
    /// 小于200MB
    /// </summary>
    LowSpeedSSD,

    /// <summary>
    /// 小于150MB
    /// </summary>
    HDD,
}
