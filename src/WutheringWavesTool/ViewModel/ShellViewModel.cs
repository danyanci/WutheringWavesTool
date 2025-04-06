using CommunityToolkit.WinUI.Animations;
using WutheringWavesTool.Pages.GamePages;
using WutheringWavesTool.Services.DialogServices;
using WutheringWavesTool.ViewModel.GameViewModels;

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

    [ObservableProperty]
    public partial Visibility BackVisibility { get; set; }
    public Controls.ImageEx Image { get; set; }
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
        //HomeNavigationService.Navigated += HomeNavigationService_Navigated;
    }

    [RelayCommand]
    void OpenMain()
    {
        this.HomeNavigationService.NavigationTo<MainGameViewModel>(
            null,
            new DrillInNavigationTransitionInfo()
        );
    }

    private void HomeNavigationService_Navigated(
        object sender,
        Microsoft.UI.Xaml.Navigation.NavigationEventArgs e
    )
    {
        if (e.SourcePageType == typeof(MainGamePage)) { }
        else { }
        if (HomeNavigationService.CanGoBack)
        {
            this.BackVisibility = Visibility.Visible;
        }
        else
        {
            this.BackVisibility = Visibility.Collapsed;
        }
        GC.Collect();
    }

    [RelayCommand]
    void BackPage()
    {
        if (HomeNavigationService.CanGoBack)
            HomeNavigationService.GoBack();
    }

    [RelayCommand]
    void Loaded() { }

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
