using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Waves.Api.Models;
using Waves.Core.Models;
using Waves.Core.Models.Enums;

namespace WutheringWavesTool.ViewModel.GameViewModels;

partial class GameViewModelBase
{
    [ObservableProperty]
    public partial string ProdBthString { get; set; }

    [ObservableProperty]
    public partial bool ProdBthEnable { get; set; }

    [ObservableProperty]
    public partial double ProdDownloadProgress { get; set; }

    [ObservableProperty]
    public partial Visibility PredDownloadBthVisibility { get; set; } = Visibility.Collapsed;

    [ObservableProperty]
    public partial Visibility PredStopBthVisibility { get; set; } = Visibility.Collapsed;

    [RelayCommand]
    async Task StartProdDownload()
    {
        var folder = await this.GameContext.GameLocalConfig.GetConfigAsync(
            GameLocalSettingName.GameLauncherBassFolder
        );
        var prodS = await GetProdSessionAsync();
        this.GameContext.StartPredDownloadGame(
            folder,
            prodS.Item1,
            prodS.Item2,
            prodS.Item1.Predownload.Version
        );
        this.ProdBthEnable = false;
        PredStopBthVisibility = Visibility.Visible;
        ProdBthString = "预下载中";
    }

    async Task<Tuple<WavesIndex, List<Resource>>> GetProdSessionAsync()
    {
        var waves = await GameContext.GetGameIndexAsync();
        var cdnUrl = waves
            .Default.CdnList.Where(p => p.P > 0)
            .OrderByDescending(p => p.P)
            .LastOrDefault();
        if (cdnUrl == null)
            return new(null, null);
        var resourceUrl = cdnUrl.Url + waves.Predownload.Resources;
        var resource = (await GameContext.GetGameResourceAsync(resourceUrl)).Resource;
        return new(waves, resource);
    }

    private async Task CheckProdInstall()
    {
        var gameFolder = await this.GameContext.GameLocalConfig.GetConfigAsync(
            GameLocalSettingName.GameLauncherBassFolder
        );
        if (gameFolder == null)
            return;
        var prodFolder = await this.GameContext.GameLocalConfig.GetConfigAsync(
            GameLocalSettingName.ProdDownloadFolderPath
        );
        var prodVersion = await this.GameContext.GameLocalConfig.GetConfigAsync(
            GameLocalSettingName.ProdDownloadVersion
        );
        var localVersion = await this.GameContext.GameLocalConfig.GetConfigAsync(
            GameLocalSettingName.LocalGameVersion
        );
        var prodDone = await this.GameContext.GameLocalConfig.GetConfigAsync(
            GameLocalSettingName.ProdDownloadFolderDone
        );
        if (prodVersion == null)
            return;
        if (bool.Parse(prodDone) == false)
        {
            return;
        }
        if (prodVersion != localVersion && Directory.Exists(prodFolder))
        {
            var prodS = await this.GetProdSessionAsync();
            if (
                prodS.Item1.Default.Version != localVersion
                && prodS.Item1.Default.Version == prodVersion
            )
            {
                TipShow.ShowMessage(
                    "检测到游戏预发布更新，已开始安装最新版本",
                    Microsoft.UI.Xaml.Controls.Symbol.Accept
                );
                Task.Run(async () =>
                {
                    this.GameContext.InstallProdGameResourceAsync(
                        gameFolder,
                        prodS.Item1,
                        prodS.Item2
                    );
                });
            }
        }
    }

    [RelayCommand]
    async Task StopProdDownload()
    {
        var status = await GameContext.GetGameStatusAsync();
        if (status.IsProdDownloading)
            this.GameContext.StopProdDownload();
        await Task.Delay(200);
        this.ProdBthEnable = true;
        PredStopBthVisibility = Visibility.Collapsed;
        ProdBthString = "开始预下载";
    }

    private async Task GameContext_GameContextProdOutput(object sender, GameContextOutputArgs args)
    {
        await AppContext.TryInvokeAsync(() =>
        {
            if (args.Type == GameContextActionType.ProdDownload)
            {
                PredDownloadBthVisibility = Visibility.Visible;
                ProdBthString = "预下载中";
                PredDownloadString = $"下载速度{args.SpeedString},进度{args.Progress}%";
                this.ProdDownloadProgress = args.Progress;
                PredStopBthVisibility = Visibility.Visible;
                this.ProdBthEnable = false;
            }
        });
    }
}
