using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using Waves.Api.Models.Communitys;
using Waves.Api.Models.Messanger;
using WavesLauncher.Core.Contracts;
using WutheringWavesTool.Common;
using WutheringWavesTool.Services.Contracts;
using WutheringWavesTool.Services.Navigations;

namespace WutheringWavesTool.ViewModel;

public partial class CommunityViewModel : ViewModelBase, IDisposable
{
    public CommunityViewModel(
        IWavesClient wavesClient,
        IAppContext<App> appContext,
        IViewFactorys viewFactorys,
        [FromKeyedServices(nameof(CommunityNavigationService))] INavigationService navigationService
    )
    {
        WavesClient = wavesClient;
        AppContext = appContext;
        ViewFactorys = viewFactorys;
        NavigationService = navigationService;
        NavigationService.Navigated += NavigationService_Navigated;
        RegisterMessanger();
    }

    private void NavigationService_Navigated(
        object sender,
        Microsoft.UI.Xaml.Navigation.NavigationEventArgs e
    )
    {
        //if (sender is Frame frame)
        //{
        //    frame.BackStack.Clear();
        //    GC.Collect();
        //}
    }

    public IWavesClient WavesClient { get; }
    public IAppContext<App> AppContext { get; }
    public IViewFactorys ViewFactorys { get; }
    public INavigationService NavigationService { get; set; }

    [ObservableProperty]
    public partial bool IsLogin { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<GameRoilDataItem> Roils { get; set; }

    [ObservableProperty]
    public partial GameRoilDataItem SelectRoil { get; set; }

    [ObservableProperty]
    public partial bool DataLoad { get; set; } = false;

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
    async Task Loaded(Frame frame = null)
    {
        if (frame != null)
            this.NavigationService.RegisterView(frame);
        this.IsLogin = (await WavesClient.IsLoginAsync());
        if (!IsLogin)
            return;
        var gamers = await WavesClient.GetWavesGamerAsync(this.CTS.Token);
        if (gamers == null || gamers.Code != 200)
            return;
        this.Roils = gamers.Data.ToObservableCollection();
        if (Roils.Count > 0)
        {
            SelectRoil = Roils[0];
            this.DataLoad = true;
        }
    }

    [RelayCommand]
    void OpenSignWindow()
    {
        var win = ViewFactorys.ShowSignWindow(this.SelectRoil);
        win.MaxHeight = 350;
        win.MaxWidth = 350;
        (win.AppWindow.Presenter as OverlappedPresenter)!.IsMaximizable = false;
        (win.AppWindow.Presenter as OverlappedPresenter)!.IsMinimizable = false;
        win.AppWindowApp.Show();
        win.Activate();
    }

    [RelayCommand]
    async Task ShowGetGeet()
    {
        await AppContext.ShowLoginDialogAsync();
    }

    public void Dispose()
    {
        this.Messenger.UnregisterAll(this);
        this.NavigationService = null;
        Roils.RemoveAll();
        Roils = null;
        this.CTS.Cancel();
    }
}
