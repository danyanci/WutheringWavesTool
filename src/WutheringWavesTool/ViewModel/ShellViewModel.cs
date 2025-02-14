using WutheringWavesTool.Services.DialogServices;

namespace WutheringWavesTool.ViewModel;

public sealed partial class ShellViewModel : ViewModelBase
{
    public INavigationService HomeNavigationService { get; }
    public ITipShow TipShow { get; }
    public IAppContext<App> AppContext { get; }
    public IWallpaperService WallpaperService { get; }
    public IDialogManager DialogManager { get; }

    [ObservableProperty]
    public partial string ServerName { get; set; }
    public ImageEx Image { get; set; }
    public Border BackControl { get; internal set; }

    public ShellViewModel(
        [FromKeyedServices(nameof(HomeNavigationService))] INavigationService homeNavigationService,
        ITipShow tipShow,
        IAppContext<App> appContext,
        IWallpaperService wallpaperService,
        [FromKeyedServices(nameof(MainDialogService))] IDialogManager dialogManager
    )
    {
        HomeNavigationService = homeNavigationService;
        TipShow = tipShow;
        AppContext = appContext;
        WallpaperService = wallpaperService;
        DialogManager = dialogManager;
        HomeNavigationService.Navigated += HomeNavigationService_Navigated;
    }

    private void HomeNavigationService_Navigated(
        object sender,
        Microsoft.UI.Xaml.Navigation.NavigationEventArgs e
    )
    {
        if (
            e.SourcePageType == typeof(MainGamePage)
            || e.SourcePageType == typeof(GlobalGamePage)
            || e.SourcePageType == typeof(BilibiliGamePage)
        )
        {
            BlurAnimationHelper.StartBlurAnimation(this.Image, 10, 0, TimeSpan.FromSeconds(0.3));
            OpacityAnimationHelper.StartAnimationHelper(this.BackControl, 0);
        }
        else
        {
            BlurAnimationHelper.StartBlurAnimation(this.Image, 0, 10, TimeSpan.FromSeconds(0.3));
            OpacityAnimationHelper.StartAnimationHelper(this.BackControl, 0.5);
        }
        GC.Collect();
    }

    [RelayCommand]
    void Loaded()
    {
        this.OpenMain();
    }

    [RelayCommand]
    void OpenCommunity()
    {
        this.HomeNavigationService.NavigationTo<CommunityViewModel>(
            "Community",
            new EntranceNavigationTransitionInfo()
        );

        ServerName = "库街区";
    }

    [RelayCommand]
    void OpenMain()
    {
        this.HomeNavigationService.NavigationTo<MainGameViewModel>(
            "MainServer",
            new DrillInNavigationTransitionInfo()
        );
        ServerName = "官服";
    }

    [RelayCommand]
    void OpenBilibili()
    {
        this.HomeNavigationService.NavigationTo<BilibiliGameViewModel>(
            "Bilibili",
            new DrillInNavigationTransitionInfo()
        );
        ServerName = "B服";
    }

    [RelayCommand]
    void OpenGlobal()
    {
        this.HomeNavigationService.NavigationTo<GlobalGameViewModel>(
            "Global",
            new DrillInNavigationTransitionInfo()
        );
        ServerName = "国际服";
    }

    [RelayCommand]
    void OpenSetting()
    {
        this.HomeNavigationService.NavigationTo<SettingViewModel>(
            "Setting",
            new DrillInNavigationTransitionInfo()
        );
    }

    [RelayCommand]
    void OpenTest()
    {
        this.HomeNavigationService.NavigationTo<TestViewModel>(
            "Setting",
            new DrillInNavigationTransitionInfo()
        );
    }
}
