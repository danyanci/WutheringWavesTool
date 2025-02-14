using WutheringWavesTool.Services.DialogServices;

namespace WutheringWavesTool.ViewModel.GameViewModels;

public sealed partial class MainGameViewModel : GameViewModelBase
{
    public MainGameViewModel(
        [FromKeyedServices(nameof(MainGameContext))] IGameContext gameContext,
        IPickersService pickersService,
        IAppContext<App> appContext,
        ITipShow tipShow,
        IWavesClient wavesClient,
        [FromKeyedServices(nameof(MainDialogService))] IDialogManager dialogManager
    )
        : base(gameContext, pickersService, appContext, tipShow, dialogManager)
    {
        WavesClient = wavesClient;
        this.RegisterMessanger();
    }

    private ObservableCollection<Content> activity;
    private ObservableCollection<Content> news;
    private ObservableCollection<Content> notice;

    [ObservableProperty]
    public partial ObservableCollection<Content> NowNews { get; set; }

    [ObservableProperty]
    public partial GamerDataModel GamerData { get; set; }

    [ObservableProperty]
    public partial Visibility GamerDataVisibility { get; set; } = Visibility.Collapsed;

    [ObservableProperty]
    public partial bool SelectBarLoad { get; set; } = false;
    public IWavesClient WavesClient { get; }

    public override async Task LoadedAfter()
    {
        var result =
            await this.GameContext.GetGameLauncherStarterAsync(
                await this.GameContext.GetGameLauncherSourceAsync(),
                true
            ) ?? null;
        if (result != null && result.Guidance != null)
        {
            this.news = result.Guidance.News.Contents.ToObservableCollection();
            this.notice = result.Guidance.Notice.Contents.ToObservableCollection();
            this.activity = result.Guidance.Activity.Contents.ToObservableCollection();
        }
        await RefreshBindAsync();
        this.SelectBarLoad = true;
    }

    private void RegisterMessanger()
    {
        this.Messenger.Register<RefreshBindUser>(this, RefreshBindUserMethod);
    }

    private async void RefreshBindUserMethod(object recipient, RefreshBindUser message)
    {
        await RefreshBindAsync();
    }

    private async Task RefreshBindAsync()
    {
        if (!(await WavesClient.IsLoginAsync()))
            return;
        var bindUser = await GameContext.GameLocalConfig.GetConfigAsync(
            GameContextExtension.BindUser
        );
        if (string.IsNullOrWhiteSpace(bindUser))
        {
            HideUserBind();
            return;
        }
        else
        {
            var gamers = await WavesClient.GetWavesGamerAsync();
            var first = gamers!.Data.Where(x => x.RoleId.ToString() == bindUser).First();
            var rr = await WavesClient.GetGamerDataAsync(first, this.CTS.Token);
            var dock = await WavesClient.GetGamerBassDataAsync(first, this.CTS.Token);
            this.GamerData = rr!;
            this.WeeklyInstCount = dock.WeeklyInstCount;
            this.WeeklyInstCountLimit = dock.WeeklyInstCountLimit;
            ShowUserBind();
        }
    }

    [ObservableProperty]
    public partial int UserRow { get; set; }

    [ObservableProperty]
    public partial int WeeklyInstCount { get; set; }

    [ObservableProperty]
    public partial int WeeklyInstCountLimit { get; set; }

    [ObservableProperty]
    public partial string WeeklyInstTitle { get; set; }

    [ObservableProperty]
    public partial int UserColumn { get; set; }

    [ObservableProperty]
    public partial int NewsRow { get; set; }

    [ObservableProperty]
    public partial int NewsColumn { get; set; }

    void ShowUserBind()
    {
        UserRow = 0;
        UserColumn = 0;
        NewsRow = 1;
        NewsColumn = 0;
        GamerDataVisibility = Visibility.Visible;
    }

    void HideUserBind()
    {
        UserRow = 0;
        UserColumn = 0;
        NewsRow = 0;
        NewsColumn = 0;
        GamerDataVisibility = Visibility.Collapsed;
    }

    [RelayCommand]
    async Task RefershGamerData()
    {
        await this.RefreshBindAsync();
    }

    [RelayCommand]
    async Task ShowBind()
    {
        if (await WavesClient.IsLoginAsync())
        {
            await this.DialogManager.ShowBindGameDataAsync("Main");
        }
        else
        {
            TipShow.ShowMessage("尚未登录或登录失效", Microsoft.UI.Xaml.Controls.Symbol.Clear);
        }
    }

    internal void SelectNews(string? v)
    {
        if (this.IsLoading)
            return;
        switch (v)
        {
            case "Dynamic":
                this.NowNews = this.activity;
                break;
            case "Subtitle":
                this.NowNews = this.notice;
                break;
            case "News":
                this.NowNews = this.news;
                break;
            default:
                break;
        }
    }

    public override async Task ShowGameResourceMethod()
    {
        await DialogManager.ShowGameResourceDialogAsync(nameof(MainGameContext));
    }

    public override async Task RemoveGameResource(DeleteGameResource resourceMessage)
    {
        if (
            resourceMessage.IsMainResource
            && resourceMessage.GameContextName == nameof(MainGameContext)
        )
        {
            await DeleteGame();
        }
        else
        {
            await DeleteProdResource();
        }
    }
}
