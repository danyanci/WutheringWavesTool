using WutheringWaves.Core.Common;
using WutheringWaves.Core.Models;

namespace WutheringWaves.Core.Contracts;

public interface IGameContext
{
    public IHttpClientService HttpClientService { get; }

    /// <summary>
    /// 获得启动器分流CDN地址
    /// </summary>
    /// <returns></returns>
    public Task<GameLauncherModel?> GetGameLauncherAsync();

    public GameLocalConfig GameLocalConfig { get; }

    /// <summary>
    /// 初始化本地配置
    /// </summary>
    /// <returns></returns>
    public Task InitializeAsync();

    /// <summary>
    /// 设置CDN分流
    /// </summary>
    /// <param name="result"></param>
    void InitializeLauncher(GameLauncherModel? result);

    /// <summary>
    /// 游戏核心状态
    /// </summary>
    /// <returns></returns>
    public Task<GameCoreStatus> GetCoreStatusAsync();

    public string GamerConfigPath { get; }
}
