using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Waves.Core.GameContext;
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
    /// 开始游戏按钮
    /// </summary>
    [ObservableProperty]
    public partial Visibility StartGameBthVisibility { get; set; }

    /// <summary>
    /// 进度条框
    /// </summary>
    [ObservableProperty]
    public partial Visibility ProgressVisibility { get; set; }

    [ObservableProperty]
    public partial Visibility UpdateVisibility { get; set; }

    [ObservableProperty]
    public partial Visibility SelectGameVisibility { get; set; }
    #endregion

    [ObservableProperty]
    public partial bool IsProgressRingActive { get; set; }

    [ObservableProperty]
    public partial double Progress { get; set; }

    [ObservableProperty]
    public partial double Maxnum { get; set; }

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
                StartGameBthVisibility = Visibility.Collapsed;
                SelectGameVisibility = Visibility.Collapsed;
                UpdateVisibility = Visibility.Visible;
                ProgressVisibility = Visibility.Visible;
                this.Progress = args.CurrentFile;
                this.Maxnum = args.MaxFile;
                this.IsProgressRingActive = false;
                VerifyOrDownloadString = $"校验文件：{args.Progress}";
            });
        }
        else if (args.Type == Waves.Core.Models.Enums.GameContextActionType.None)
        {
            await AppContext.TryInvokeAsync(() =>
            {
                StartGameBthVisibility = Visibility.Collapsed;
                SelectGameVisibility = Visibility.Collapsed;
                UpdateVisibility = Visibility.Visible;
                ProgressVisibility = Visibility.Visible;
                VerifyOrDownloadString = $"校验完成";
            });
            Debug.WriteLine("当前无动作");
            var result = await GameContext.GetGameStausAsync();
            await AppContext.TryInvokeAsync(() =>
            {
                if (result.IsLauncheExists)
                {
                    StartGameBthVisibility = Visibility.Visible;
                    UpdateVisibility = Visibility.Collapsed;
                }
            });
        }
    }

    [RelayCommand]
    async Task Loaded()
    {
        IsLoading = true;
        var status = await GameContext.GetGameStausAsync(this.CTS.Token);
        if (!status.IsLauncheExists)
        {
            StartGameBthVisibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            UpdateVisibility = Microsoft.UI.Xaml.Visibility.Visible;
            SelectGameVisibility = Visibility.Visible;
            ProgressVisibility = Visibility.Collapsed;
        }
        else
        {
            StartGameBthVisibility = Microsoft.UI.Xaml.Visibility.Visible;
            UpdateVisibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            SelectGameVisibility = Visibility.Collapsed;
        }
        await LoadedAfter();
        IsLoading = false;
    }

    [RelayCommand]
    async Task OpenSelectGameFolder()
    {
        var folder = await PickersService.GetFileOpenPicker(new() { ".exe" });
        GameContext.StartVerifyGame(folder.Path);
    }

    public virtual Task LoadedAfter()
    {
        return Task.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing) { }

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
