using WutheringWaves.Core.Common;
using WutheringWaves.Core.Contracts;
using WutheringWaves.Core.Models;

namespace WutheringWaves.Core.GameContext;

public partial class GameContextBase : IGameContext
{
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
    public CdnList Cdn { get; private set; }

    public void InitializeCdn(GameLauncherModel? result)
    {
        if (result != null)
        {
            this.Cdn = result.Default.CdnList.Where(x => x.P != 0).OrderBy(x => x.P).First();
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
}
