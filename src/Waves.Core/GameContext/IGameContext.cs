using Waves.Api.Models;
using Waves.Core.Contracts;
using Waves.Core.Models;

namespace Waves.Core.GameContext;

public interface IGameContext
{
    public IHttpClientService HttpClientService { get; set; }

    public IGameContextDownloadCache GameContextDownloadCahce { get; }
    public Task InitAsync();
    public string ContextName { get; }
    public bool IsNext { get; }
    event GameContextOutputDelegate GameContextOutput;
    event GameContextProdOutputDelegate GameContextProdOutput;
    public string GamerConfigPath { get; internal set; }
    GameLocalConfig GameLocalConfig { get; }

    /// <summary>
    /// 是否可以启动
    /// </summary>
    public bool IsLaunch { get; }

    /// <summary>
    /// 获得上下文状态
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<GameContextStatus> GetGameStatusAsync(CancellationToken token = default);
    void StartVerifyGame(string folder);

    void StartDownloadGame(string folder, WavesIndex waves, GameResource resource, bool isNew);

    void StartPredDownloadGame(
        string folder,
        WavesIndex index,
        List<Resource> resources,
        string version
    );
    Task InstallProdGameResourceAsync(string folder, WavesIndex index);
    Task CancelDownloadAsync();

    Task<WavesIndex> GetGameIndexAsync(CancellationToken token = default);

    Task<GameResource> GetGameResourceAsync(string resourceUrl, CancellationToken token = default);
    Task ClearGameResourceAsync();

    Task<bool> CheckUpdateAsync(CancellationToken token = default);

    #region 启动游戏
    Task StartLauncheAsync();
    Task StopGameVerify();

    #endregion

    #region Core Config
    Task<GameContextConfig> ReadContextConfigAsync(CancellationToken token = default);

    Task<bool> SetDx11LauncheAsync(bool value, CancellationToken token = default);
    Task<bool> SetLimitSpeedAsync(int value, CancellationToken token = default);
    #endregion

    #region WebApi
    Task<LauncherHeader?> GetLauncherHeaderAsync(CancellationToken token = default);

    Task<GameLauncherSource> GetGameLauncherSourceAsync(CancellationToken token = default);

    Task<GameLauncherStarter?> GetGameLauncherStarterAsync(
        GameLauncherSource launcheSource,
        bool ishans,
        CancellationToken token = default
    );
    void StopProdDownload();
    Task DeleteGameProdResourceAsync();
    #endregion
}
