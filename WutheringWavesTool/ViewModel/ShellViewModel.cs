using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Media.Animation;
using WutheringWavesTool.Common;
using WutheringWavesTool.Services.Contracts;
using WutheringWavesTool.Services.Navigations;
using WutheringWavesTool.ViewModel.GameViewModels;

namespace WutheringWavesTool.ViewModel;

public sealed partial class ShellViewModel : ViewModelBase
{
    public ShellViewModel(
        [FromKeyedServices(nameof(HomeNavigationService))] INavigationService homeNavigationService
    )
    {
        HomeNavigationService = homeNavigationService;
    }

    [RelayCommand]
    void OpenMain()
    {
        this.HomeNavigationService.NavigationTo<MainGameViewModel>(
            "MainServer",
            new DrillInNavigationTransitionInfo()
        );
    }

    [RelayCommand]
    void OpenBilibili()
    {
        this.HomeNavigationService.NavigationTo<BilibiliGameViewModel>(
            "Bilibili",
            new DrillInNavigationTransitionInfo()
        );
    }

    [RelayCommand]
    void OpenGlobal()
    {
        this.HomeNavigationService.NavigationTo<GlobalGameViewModel>(
            "Global",
            new DrillInNavigationTransitionInfo()
        );
    }

    public INavigationService HomeNavigationService { get; }
}
