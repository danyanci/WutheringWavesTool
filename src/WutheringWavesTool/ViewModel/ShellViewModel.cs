using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Media.Animation;
using WutheringWavesTool.Common;
using WutheringWavesTool.Services.Contracts;
using WutheringWavesTool.ViewModel.GameViewModels;

namespace WutheringWavesTool.ViewModel;

public sealed partial class ShellViewModel : ViewModelBase
{
    public INavigationService HomeNavigationService { get; }
    public ITipShow TipShow { get; }
    public IAppContext<App> AppContext { get; }

    [ObservableProperty]
    public partial string ServerName { get; set; }

    public ShellViewModel(
        [FromKeyedServices(nameof(HomeNavigationService))] INavigationService homeNavigationService,
        ITipShow tipShow,
        IAppContext<App> appContext
    )
    {
        HomeNavigationService = homeNavigationService;
        TipShow = tipShow;
        AppContext = appContext;
        HomeNavigationService.Navigated += HomeNavigationService_Navigated;
    }

    private void HomeNavigationService_Navigated(
        object sender,
        Microsoft.UI.Xaml.Navigation.NavigationEventArgs e
    )
    {
        //HomeNavigationService.ClearHistory();
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
}
