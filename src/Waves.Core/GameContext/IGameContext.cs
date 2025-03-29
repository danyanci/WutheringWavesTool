using Waves.Api.Models;
using Waves.Core.Contracts;
using Waves.Core.Models;
using Waves.Core.Models.Downloader;

namespace Waves.Core.GameContext;

public interface IGameContext
{
    public IHttpClientService HttpClientService { get; set; }

    public Task InitAsync();
    public string ContextName { get; }
    event GameContextOutputDelegate GameContextOutput;
    event GameContextProdOutputDelegate GameContextProdOutput;
    public string GamerConfigPath { get; internal set; }
    GameLocalConfig GameLocalConfig { get; }

    Task<FileVersion> GetLocalDLSSAsync();
    Task<FileVersion> GetLocalDLSSGenerateAsync();
    Task<FileVersion> GetLocalXeSSGenerateAsync();

    public Type ContextType { get; }

    #region Launcher
    Task<GameLauncherSource?> GetGameLauncherSourceAsync(CancellationToken token = default);
    #endregion

    #region Core
    Task<GameContextStatus> GetGameContextStatusAsync(CancellationToken token = default);
    #endregion

    #region Downloader
    Task<IndexGameResource> GetGameResourceAsync(
        ResourceDefault ResourceDefault,
        CancellationToken token = default
    );

    Task StartDownloadTaskAsync(string folder, GameLauncherSource? source);
    #endregion
}
