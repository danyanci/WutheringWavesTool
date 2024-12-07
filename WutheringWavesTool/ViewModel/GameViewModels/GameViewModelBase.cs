using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
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
    public partial double Maxnum { get; set; }

    [ObservableProperty]
    public partial string DownloadIcon { get; set; }

    [ObservableProperty]
    public partial string DownloadText { get; set; }

    [ObservableProperty]
    public partial string VerifyOrDownloadString { get; set; }
    public IGameContext GameContext { get; }
    public IPickersService PickersService { get; }
    public IAppContext<App> AppContext { get; }

    public GameViewModelBase(
        IGameContext gameContext,
        IPickersService pickersService,
        IAppContext<App> appContext
    )
    {
        GameContext = gameContext;
        PickersService = pickersService;
        AppContext = appContext;
        this.GameContext.GameContextOutput += GameContext_GameContextOutput;
    }

    private async void GameContext_GameContextOutput(
        object sender,
        Waves.Core.Models.GameContextOutputArgs args
    )
    {
        if (args.Type == Waves.Core.Models.Enums.GameContextActionType.Verify)
        {
            await AppContext.TryInvokeAsync(() =>
            {
                ShowDownload();
                this.Progress = args.CurrentFile;
                this.Maxnum = args.MaxFile;
                this.IsProgressRingActive = false;
                VerifyOrDownloadString = $"校验文件：{args.Progress}%";
            });
        }
        else if (args.Type == Waves.Core.Models.Enums.GameContextActionType.Download)
        {
            await AppContext.TryInvokeAsync(() =>
            {
                ShowDownload();
                this.Progress = args.Progress;
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
                this.Progress = args.Progress;
                this.IsProgressRingActive = false;
                VerifyOrDownloadString = $"正在移动文件:{args.CurrentFile}/{args.MaxFile}";
                this.Maxnum = 100;
                return;
            });
        }
        else if (args.Type == Waves.Core.Models.Enums.GameContextActionType.None)
        {
            var result = await GameContext.GetGameStausAsync();
            await AppContext.TryInvokeAsync(() =>
            {
                if (result.IsDownloadComplete && result.IsSelectDownloadFolder)
                {
                    ShowStartGame();
                }
                else if (!result.IsDownloadComplete && !result.IsSelectDownloadFolder)
                {
                    ShowSelectFolder();
                }
            });
        }
    }

    [RelayCommand]
    async Task Loaded()
    {
        IsLoading = true;
        var status = await GameContext.GetGameStausAsync(this.CTS.Token);
        if (!status.IsSelectDownloadFolder)
        {
            ShowSelectFolder();
        }
        if (status.IsSelectDownloadFolder && status.IsDownloadComplete)
        {
            ShowStartGame();
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
        await LoadedAfter();
        IsLoading = false;
    }

    [RelayCommand]
    async Task SwitchDownload()
    {
        var status = await GameContext.GetGameStausAsync(this.CTS.Token);
        if (status.IsDownload)
        {
            await GameContext.CancelDownloadAsync();
            DownloadIcon = "\uE768";
            DownloadText = "继续下载";
        }
        else
        {
            GameContext.StartDownloadGame(
                await GameContext.GameLocalConfig.GetConfigAsync(
                    GameLocalSettingName.GameLauncherBassFolder
                ),
                false
            );

            DownloadIcon = "\uE769";
            DownloadText = "暂停下载";
        }
    }

    [RelayCommand]
    async Task DeleteGameResource()
    {
        await this.GameContext.ClearGameResourceAsync();
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
        GameContext.StartDownloadGame(folder.Path, true);
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
