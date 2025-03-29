using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using CommunityToolkit.Mvvm.Input;
using SqlSugar;
using Waves.Api.Models;
using Waves.Core.Common;
using Waves.Core.Contracts;
using Waves.Core.Models;
using Waves.Core.Models.Enums;

namespace Waves.Core.GameContext;

public abstract partial class GameContextBase : IGameContext
{
    #region _filed

    private bool isLimtSpeed;
    private CancellationTokenSource _downloadCTS;
    private CancellationTokenSource _prodDowloadCTS;
    private CancellationTokenSource _clearCTS;
    private CancellationTokenSource verifyCts;
    private CancellationTokenSource _installProdCTS;
    #endregion

    #region Property
    public IHttpClientService HttpClientService { get; set; }
    public GameApiContextConfig Config { get; private set; }
    public string ContextName { get; }
    public string GamerConfigPath { get; set; }

    public GameLocalConfig GameLocalConfig { get; private set; }

    public bool IsDx11Launche { get; private set; }

    public bool IsLimitSpeed
    {
        get => isLimtSpeed;
        set { this.isLimtSpeed = value; }
    }

    public int SpeedValue { get; private set; }

    public Process NowProcess { get; private set; }

    public virtual Type ContextType { get; }
    #endregion


    internal GameContextBase(GameApiContextConfig config, string contextName)
    {
        Config = config;
        ContextName = contextName;
    }

    public virtual async Task InitAsync()
    {
        this.HttpClientService.BuildClient();
        Directory.CreateDirectory(GamerConfigPath);
        this.GameLocalConfig = new GameLocalConfig();
        GameLocalConfig.SettingPath = GamerConfigPath + "\\Settings.db";
        await InitSettingAsync();
    }

    private async Task InitSettingAsync()
    {
        var config = await this.ReadContextConfigAsync();
        if (config.LimitSpeed > 0)
        {
            this.SpeedValue = config.LimitSpeed;
            this.IsLimitSpeed = true;
        }
    }

    public async Task<GameContextStatus> GetGameContextStatusAsync(
        CancellationToken token = default
    )
    {
        GameContextStatus status = new GameContextStatus();
        var gameBaseFolder = await GameLocalConfig.GetConfigAsync(
            GameLocalSettingName.GameLauncherBassFolder
        );
        if (gameBaseFolder == null)
        {
            status.IsGameExists = false;
        }
        return status;
    }
}
