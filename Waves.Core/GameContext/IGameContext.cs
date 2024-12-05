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
    Task<GameContextStatus> GetGameStausAsync(CancellationToken token = default);
    void StartVerifyGame(string folder);

    void StartDownloadGame(string folder);

    Task<WavesIndex> GetGameIndexAsync(CancellationToken token = default);

    Task<GameResource> GetGameResourceAsync(string resourceUrl, CancellationToken token = default);
}
