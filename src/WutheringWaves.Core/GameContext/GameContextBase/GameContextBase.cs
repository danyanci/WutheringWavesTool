using WutheringWaves.Core.Common;
using WutheringWaves.Core.Contracts;
using WutheringWaves.Core.Models;

namespace WutheringWaves.Core.GameContext;

public partial class GameContextBase : IGameContext
{
    private GameLauncherModel _launcherModel;

    public GameContextBase(GameApiContextConfig config, string contextName)
    {
        Config = config;
        ContextName = contextName;
    }

    public GameApiContextConfig Config { get; }
    public string ContextName { get; }

    public string GamerConfigPath { get; internal set; }
    public IHttpClientService HttpClientService { get; internal set; }

    public GameLocalConfig GameLocalConfig { get; private set; }

    public void InitializeLauncher(GameLauncherModel? result)
    {
        if (result != null)
        {
            this._launcherModel = result;
            return;
        }
        throw new ArgumentException("请求游戏数据错误");
    }

    public async Task InitializeAsync()
    {
        this.HttpClientService.BuildClient();
        Directory.CreateDirectory(GamerConfigPath);
        this.GameLocalConfig = new GameLocalConfig();
        GameLocalConfig.SettingPath = GamerConfigPath + "\\Settings.db";
        await InitSettingAsync();
    }

    private async Task InitSettingAsync()
    {
        await Task.CompletedTask;
    }

    public async Task<GameCoreStatus> GetCoreStatusAsync()
    {
        return await Task.FromResult<GameCoreStatus>(new GameCoreStatus());
    }
}
