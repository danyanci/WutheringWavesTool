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
        RegisterMessanger();
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
        this.Messenger.Register<ShowRoleData>(this, ShowRoleMethod);
    }

    private async void ShowRoleMethod(object recipient, ShowRoleData message)
    {
        var Roles = await WavesClient.GetGamerRoilDetily(
            this.SelectRoil,
            message.Id,
            this.CTS.Token
        );
        if (Roles == null)
            return;
        ViewFactorys.ShowRoleDataWindow(Roles).AppWindow.Show();
    }

    private async void LoginMessangerMethod(object recipient, LoginMessanger message)
    {
        await LoadedAsync();
    }

    [RelayCommand]
    async Task UnLogin()
    {
        AppSettings.Token = "";
        AppSettings.TokenId = "";
        await LoadedAsync();
    }

    [RelayCommand]
    async Task LoadedAsync(Frame frame = null)
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
        var skin = WavesClient.GetGamerSkinAsync(this.SelectRoil, this.CTS.Token);
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
    }

    [RelayCommand]
    void OpenPlayerRecordWindow()
    {
        var win = ViewFactorys.ShowPlayerRecordWindow();
        (win.AppWindow.Presenter as OverlappedPresenter)!.IsMaximizable = false;
        (win.AppWindow.Presenter as OverlappedPresenter)!.IsMinimizable = false;
        win.SystemBackdrop = new MicaBackdrop();
        win.AppWindowApp.Show();
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
