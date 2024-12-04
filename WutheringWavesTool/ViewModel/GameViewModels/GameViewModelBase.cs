using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Waves.Core.GameContext;
using Windows.Devices.WiFi;
using WutheringWavesTool.Common;
using WutheringWavesTool.Services.Contracts;

namespace WutheringWavesTool.ViewModel.GameViewModels;

public abstract partial class GameViewModelBase : ViewModelBase, IDisposable
{
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

    private void GameContext_GameContextOutput(
        object sender,
        Waves.Core.Models.GameContextOutputArgs args
    )
    {
        if (args.Type == Waves.Core.Models.Enums.GameContextActionType.Verify)
        {
            Debug.WriteLine(
                $"校验进度{args.Progress},当前文件{args.CurrentFile},总文件{args.MaxFile}"
            );
        }
        else if (args.Type == Waves.Core.Models.Enums.GameContextActionType.None)
        {
            Debug.WriteLine("当前无动作");
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
            SelectLocalVisibility = Microsoft.UI.Xaml.Visibility.Visible;
        }
        else
        {
            StartGameBthVisibility = Microsoft.UI.Xaml.Visibility.Visible;
            SelectLocalVisibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }
        await LoadedAfter();
        IsLoading = false;
    }

    [ObservableProperty]
    public partial Visibility ProgressVisibility { get; set; }

    [ObservableProperty]
    public partial Visibility SelectGameVisibility { get; set; }

    [RelayCommand]
    async Task OpenSelectGameFolder()
    {
        var folder = await PickersService.GetFileOpenPicker(new() { ".exe" });
        GameContext.StartVerifyGame(Path.GetDirectoryName(folder.Path));
    }

    public virtual Task LoadedAfter()
    {
        return Task.CompletedTask;
    }

    private bool disposedValue;

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial Visibility StartGameBthVisibility { get; set; }

    [ObservableProperty]
    public partial Visibility SelectLocalVisibility { get; set; }

    public IGameContext GameContext { get; }
    public IPickersService PickersService { get; }
    public IAppContext<App> AppContext { get; }

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
