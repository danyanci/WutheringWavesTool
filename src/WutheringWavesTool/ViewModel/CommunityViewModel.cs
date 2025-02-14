using WutheringWavesTool.Services.DialogServices;

namespace WutheringWavesTool.ViewModel;

public partial class CommunityViewModel : ViewModelBase, IDisposable
{
    public CommunityViewModel(
        IWavesClient wavesClient,
        IViewFactorys viewFactorys,
        [FromKeyedServices(nameof(CommunityNavigationService))]
            INavigationService navigationService,
        [FromKeyedServices(nameof(MainDialogService))] IDialogManager dialogManager
    )
    {
        WavesClient = wavesClient;
        ViewFactorys = viewFactorys;
        NavigationService = navigationService;
        DialogManager = dialogManager;
        RegisterMessanger();
    }

    public IWavesClient WavesClient { get; }
    public IAppContext<App> AppContext { get; }
    public IViewFactorys ViewFactorys { get; }
    public INavigationService NavigationService { get; set; }
    public IDialogManager DialogManager { get; }

    [ObservableProperty]
    public partial bool IsLogin { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<GameRoilDataWrapper> Roils { get; set; }

    [ObservableProperty]
    public partial GameRoilDataWrapper SelectRoil { get; set; }

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
            this.SelectRoil.Item,
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
        this.Roils = await FormatRoilAsync(gamers.Data);
        if (Roils.Count > 0)
        {
            SelectRoil = Roils[0];
            await WavesClient.RefreshGamerDataAsync(this.SelectRoil.Item, this.CTS.Token);
            var Roildock = await WavesClient.GetGamerBassDataAsync(SelectRoil.Item, this.CTS.Token);
            var skin = WavesClient.GetGamerSkinAsync(this.SelectRoil.Item, this.CTS.Token);
            this.DataLoad = true;
        }
    }

    async Task<ObservableCollection<GameRoilDataWrapper>> FormatRoilAsync(
        List<GameRoilDataItem> roilDataItems
    )
    {
        ObservableCollection<GameRoilDataWrapper> values = new();
        foreach (var item in roilDataItems)
        {
            GameRoilDataWrapper value = new GameRoilDataWrapper(item);
            var level = await WavesClient.GetGamerBassDataAsync(item, this.CTS.Token);
            value.GameLevel = level!.Level;
            values.Add(value);
        }
        return values;
    }

    [RelayCommand]
    void OpenSignWindow()
    {
        var win = ViewFactorys.ShowSignWindow(this.SelectRoil.Item);
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
        await DialogManager.ShowLoginDialogAsync();
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
