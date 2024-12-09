using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Waves.Api.Models;
using Waves.Core.GameContext;
using Waves.Core.Models;
using Windows.Devices.WiFi;
using WutheringWavesTool.Common;
using WutheringWavesTool.Services.Contracts;

namespace WutheringWavesTool.ViewModel.GameViewModels;

public abstract partial class GameViewModelBase : ViewModelBase, IDisposable
{
    private bool disposedValue;

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial WavesIndex WavesIndex { get; private set; }

    #region Visibility
    /// <summary>
    /// 进度条框
    /// </summary>
    [ObservableProperty]
    public partial Visibility ProgressVisibility { get; set; } = Visibility.Collapsed;

    [ObservableProperty]
    public partial Visibility SelectGameVisibility { get; set; } = Visibility.Collapsed;

    [ObservableProperty]
    public partial Visibility StartGameVisibility { get; set; } = Visibility.Collapsed;
    #endregion

    [ObservableProperty]
    public partial bool IsProgressRingActive { get; set; }

    [ObservableProperty]
    public partial double Progress { get; set; }

    [ObservableProperty]
    public partial string SpeedString { get; set; }

    [ObservableProperty]
    public partial string SurplusValue { get; set; }

    [ObservableProperty]
    public partial string LastTime { get; set; }

    [ObservableProperty]
    public partial double Maxnum { get; set; }

    [ObservableProperty]
    public partial string DownloadIcon { get; set; }

    [ObservableProperty]
    public partial bool DownloadGameEnable { get; set; } = true;

    [ObservableProperty]
    public partial string DownloadText { get; set; }

    [ObservableProperty]
    public partial string WorkType { get; set; }

    [ObservableProperty]
    public partial string VerifyOrDownloadString { get; set; }
    public IGameContext GameContext { get; }
    public IPickersService PickersService { get; }
    public IAppContext<App> AppContext { get; }
    public ITipShow TipShow { get; }

    [ObservableProperty]
    public partial string LastVerision { get; set; }

    [ObservableProperty]
    public partial bool LauncherGameEnable { get; set; }

    public GameViewModelBase(
        IGameContext gameContext,
        IPickersService pickersService,
        IAppContext<App> appContext,
        ITipShow tipShow
    )
    {
        GameContext = gameContext;
        PickersService = pickersService;
        AppContext = appContext;
        TipShow = tipShow;
        this.GameContext.GameContextOutput += GameContext_GameContextOutput;
    }

    private async Task GameContext_GameContextOutput(
        object sender,
        Waves.Core.Models.GameContextOutputArgs args
    )
    {
        if (args.Type == Waves.Core.Models.Enums.GameContextActionType.Verify)
        {
            await AppContext.TryInvokeAsync(() =>
            {
                ShowDownload();
                this.Progress = args.Progress;
                this.SurplusValue = args.RemainingTime;
                this.SpeedString = "0MB";
                this.WorkType = "校验";
                this.Maxnum = 100;
                this.LastTime = "0GB";
                DownloadGameEnable = false;
                DownloadText = "校验中";
                this.IsProgressRingActive = false;
                VerifyOrDownloadString = $"校验文件：{args.Progress}%";
            });
        }
        else if (args.Type == Waves.Core.Models.Enums.GameContextActionType.Download)
        {
            await AppContext.TryInvokeAsync(() =>
            {
                DownloadGameEnable = true;
                DownloadIcon = "\uE769";
                DownloadText = "暂停下载";
                ShowDownload();
                this.Progress = args.Progress;
                this.SurplusValue = args.RemainingTime;
                this.SpeedString = args.SpeedString;
                this.Maxnum = args.MaxFile;
                this.WorkType = "下载";
                this.LastTime = $"{(args.MaxSize - args.CurrentSize) / 1024 / 1024 / 1024:F2}GB";
                this.IsProgressRingActive = false;
                VerifyOrDownloadString =
                    $"下载进度：{this.Progress}%，速度:{args.SpeedString}，剩余:{(args.MaxSize - args.CurrentSize) / 1024 / 1024 / 1024:F2}GB，预计:{args.RemainingTime}";
                this.Maxnum = 100;
                return;
            });
        }
        else if (args.Type == Waves.Core.Models.Enums.GameContextActionType.Clear)
        {
            await AppContext.TryInvokeAsync(() =>
            {
                ShowDownload();
                DownloadGameEnable = false;
                this.Progress = args.CurrentFile;
                this.SurplusValue = args.RemainingTime;
                this.SpeedString = args.SpeedString;
                this.Maxnum = args.MaxFile;
                this.WorkType = "清理";
                this.LastTime = $"0GB";
                this.IsProgressRingActive = false;
                VerifyOrDownloadString = $"正在移动文件:{args.CurrentFile}/{args.MaxFile}";
                this.Maxnum = 100;
                return;
            });
        }
        else if (
            args.Type == Waves.Core.Models.Enums.GameContextActionType.None
            || args.Type == Waves.Core.Models.Enums.GameContextActionType.Error
        )
        {
            await AppContext.TryInvokeAsync(() =>
            {
                if (args.Type == Waves.Core.Models.Enums.GameContextActionType.Error)
                {
                    this.TipShow.ShowMessage($"{args.ErrorMessage}", Symbol.Clear);
                    VerifyOrDownloadString = $"下载或校验出现网络问题";
                    this.IsProgressRingActive = false;
                }
            });
            await AppContext.TryInvokeAsync(async () =>
            {
                await RefreshStatus();
            });
        }
    }

    [RelayCommand]
    async Task Loaded()
    {
        IsLoading = true;
        this.WavesIndex = await GameContext.GetGameIndexAsync(this.CTS.Token);
        await RefreshStatus();
        await LoadedAfter();
        IsLoading = false;
    }

    async Task RefreshStatus()
    {
        var status = await GameContext.GetGameStatusAsync(this.CTS.Token);
        if (!status.IsSelectDownloadFolder)
        {
            this.LastVerision = WavesIndex.Default.ResourceChunk.LastVersion;
            ShowSelectFolder();
        }
        if (status.IsSelectDownloadFolder && status.IsDownloadComplete)
        {
            ShowStartGame();
            if (status.IsLauncherGame)
                LauncherGameEnable = false;
            else
                LauncherGameEnable = true;
        }
        if (!status.IsDownloadComplete)
        {
            if (status.IsSelectDownloadFolder)
            {
                DownloadIcon = "\uE768";
                DownloadText = "继续下载";
                ProgressVisibility = Visibility.Visible;
                ShowDownload();
            }
        }
        if (status.IsVerify)
        {
            ShowDownload();
            this.IsProgressRingActive = true;
            VerifyOrDownloadString = "读取校验状态";
            DownloadIcon = "\uE769";
            DownloadText = "暂停下载";
            await LoadedAfter();
            IsLoading = false;
            return;
        }
        if (status.IsDownload)
        {
            ShowDownload();
            this.IsProgressRingActive = true;
            VerifyOrDownloadString = "读取下载状态";
            DownloadIcon = "\uE769";
            DownloadText = "暂停下载";
            await LoadedAfter();
            IsLoading = false;
            return;
        }
    }

    [RelayCommand]
    async Task SwitchDownload()
    {
        var status = await GameContext.GetGameStatusAsync(this.CTS.Token);
        if (status.IsDownload)
        {
            await GameContext.CancelDownloadAsync();
            DownloadIcon = "\uE768";
            DownloadText = "继续下载";
        }
        else
        {
            await StartDown(
                await GameContext.GameLocalConfig.GetConfigAsync(
                    GameLocalSettingName.GameLauncherBassFolder
                )
            );
            DownloadIcon = "\uE769";
            DownloadText = "暂停下载";
        }
    }

    [RelayCommand]
    async Task VerifyGame()
    {
        var status = await GameContext.GetGameStatusAsync(this.CTS.Token);
        if (status.IsDownloadComplete && status.IsSelectDownloadFolder)
        {
            this.ShowDownload();
            this.DownloadGameEnable = false;
            var exeFile = await GameContext.GameLocalConfig.GetConfigAsync(
                GameLocalSettingName.GameLauncherBassProgram
            );
            GameContext.StartVerifyGame(exeFile);
        }
    }

    [RelayCommand]
    async Task CheckGameUpdate()
    {
        var update = await GameContext.CheckUpdateAsync(this.CTS.Token);
        if (update)
        {
            this.TipShow.ShowMessage("无任何更新", Symbol.Accept);
        }
        else
        {
            this.TipShow.ShowMessage("开始自动更新", Symbol.Accept);
        }
    }

    [RelayCommand]
    async Task LauncheGameAsync()
    {
        await GameContext.StartLauncheAsync();
        await this.RefreshStatus();
    }

    [RelayCommand]
    async Task DeleteGameResource()
    {
        var status = await this.GameContext.GetGameStatusAsync();
        if (status.IsDownload)
        {
            await this.GameContext.ClearGameResourceAsync();
        }
        else if (status.IsVerify)
        {
            await this.GameContext.StopGameVerify();
        }
    }

    private void ShowDownload()
    {
        ProgressVisibility = Visibility.Visible;
        SelectGameVisibility = Visibility.Collapsed;
        StartGameVisibility = Visibility.Collapsed;
    }

    private void ShowSelectFolder()
    {
        SelectGameVisibility = Visibility.Visible;
        ProgressVisibility = Visibility.Collapsed;
        StartGameVisibility = Visibility.Collapsed;
    }

    private void ShowStartGame()
    {
        SelectGameVisibility = Visibility.Collapsed;
        ProgressVisibility = Visibility.Collapsed;
        StartGameVisibility = Visibility.Visible;
    }

    [RelayCommand]
    async Task OpenSelectGameFolder()
    {
        var folder = await PickersService.GetFileOpenPicker(new() { ".exe" });
        IsProgressRingActive = true;
        GameContext.StartVerifyGame(folder.Path);
    }

    [RelayCommand]
    async Task OpenSelectGameDownloadFolder()
    {
        var folder = await PickersService.GetFolderPicker();
        IsProgressRingActive = true;
        await StartDown(folder.Path);
    }

    async Task StartDown(string folder)
    {
        var launcherIndex = await GameContext.GetGameIndexAsync();
        var Cdn = launcherIndex
            .Default.CdnList.Where(p => p.P > 0)
            .OrderByDescending(p => p.P)
            .LastOrDefault();
        if (Cdn == null)
        {
            TipShow.ShowMessage("CDN解析错误", Symbol.Clear);
            return;
        }
        var url = Cdn!.Url + launcherIndex.Default.ResourcesBasePath;
        var resourceUrl = Cdn.Url + launcherIndex.Default.Resources;
        var resource = await GameContext.GetGameResourceAsync(url);

        GameContext.StartDownloadGame(folder, launcherIndex, resource, true);
    }

    [RelayCommand]
    async Task RefreshGame()
    {
        GameContext.StartVerifyGame(
            await this.GameContext.GameLocalConfig.GetConfigAsync(
                GameLocalSettingName.GameLauncherBassProgram
            )
        );
    }

    public virtual Task LoadedAfter()
    {
        return Task.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                this.GameContext.GameContextOutput -= GameContext_GameContextOutput;
            }

            disposedValue = true;
        }
    }

    // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
    // ~GameViewModelBase()
    // {
    //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
