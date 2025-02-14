using WutheringWavesTool.Services.DialogServices;

namespace WutheringWavesTool.ViewModel.GameViewModels;

public sealed partial class GlobalGameViewModel : GameViewModelBase
{
    public GlobalGameViewModel(
        [FromKeyedServices(nameof(GlobalGameContext))] IGameContext gameContext,
        IPickersService pickersService,
        IAppContext<App> appContext,
        ITipShow tipShow,
        [FromKeyedServices(nameof(MainDialogService))] IDialogManager dialogManager
    )
        : base(gameContext, pickersService, appContext, tipShow, dialogManager) { }

    private ObservableCollection<Content> activity;
    private ObservableCollection<Content> news;
    private ObservableCollection<Content> notice;

    [ObservableProperty]
    public partial ObservableCollection<Content> NowNews { get; set; }

    [ObservableProperty]
    public partial bool SelectBarLoad { get; set; } = false;

    public override async Task LoadedAfter()
    {
        var result =
            await this.GameContext.GetGameLauncherStarterAsync(
                await this.GameContext.GetGameLauncherSourceAsync(),
                false
            ) ?? null;
        if (result != null && result.Guidance != null)
        {
            this.news = result.Guidance.News.Contents.ToObservableCollection();
            this.notice = result.Guidance.Notice.Contents.ToObservableCollection();
            this.activity = result.Guidance.Activity.Contents.ToObservableCollection();
        }
        this.SelectBarLoad = true;
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
        await DialogManager.ShowGameResourceDialogAsync(nameof(GlobalGameContext));
    }

    public override Task RemoveGameResource(DeleteGameResource resourceMessage)
    {
        throw new NotImplementedException();
    }
}
