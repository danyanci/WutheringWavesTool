using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Waves.Api.Models.Communitys;
using Waves.Api.Models.Messanger;
using WavesLauncher.Core.Contracts;
using WinUIEx;
using WutheringWavesTool.Common;
using WutheringWavesTool.Services.Contracts;

namespace WutheringWavesTool.ViewModel;

public partial class CommunityViewModel : ViewModelBase, IDisposable
{
    public CommunityViewModel(IWavesClient wavesClient, IAppContext<App> appContext)
    {
        WavesClient = wavesClient;
        AppContext = appContext;
        RegisterMessanger();
    }

    public IWavesClient WavesClient { get; }
    public IAppContext<App> AppContext { get; }

    [ObservableProperty]
    public partial bool IsLogin { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<GameRoilDataItem> Roils { get; set; }

    [ObservableProperty]
    public partial GameRoilDataItem SelectRoil { get; set; }

    private void RegisterMessanger()
    {
        this.Messenger.Register<LoginMessanger>(this, LoginMessangerMethod);
    }

    private async void LoginMessangerMethod(object recipient, LoginMessanger message)
    {
        await Loaded();
    }

    [RelayCommand]
    async Task UnLogin()
    {
        AppSettings.Token = "";
        AppSettings.TokenId = "";
        await Loaded();
    }

    [RelayCommand]
    async Task Loaded()
    {
        this.IsLogin = await WavesClient.IsLoginAsync();
        if (!IsLogin)
            return;
        var gamers = await WavesClient.GetWavesGamerAsync(this.CTS.Token);
        if (gamers == null || gamers.Code != 200)
            return;
        this.Roils = gamers.Data.ToObservableCollection();
        if (Roils.Count > 0)
        {
            SelectRoil = Roils[0];
        }
    }

    [RelayCommand]
    async Task ShowGetGeet()
    {
        await AppContext.ShowLoginDialogAsync();
    }

    public void Dispose()
    {
        this.Messenger.UnregisterAll(this);
        this.CTS.Cancel();
    }
}
