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
        if (waves.Predownload == null)
            return new(waves, null);
        var resourceUrl = cdnUrl.Url + waves.Predownload.Resources;
        var resource = (await GameContext.GetGameResourceAsync(resourceUrl)).Resource;
        return new(waves, resource);
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
