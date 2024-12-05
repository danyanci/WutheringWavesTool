using System;
using Microsoft.Extensions.DependencyInjection;
using Waves.Core;
using WutheringWavesTool.Pages;
using WutheringWavesTool.Services;
using WutheringWavesTool.Services.Contracts;
using WutheringWavesTool.Services.Navigations;
using WutheringWavesTool.ViewModel;
using WutheringWavesTool.ViewModel.GameViewModels;

namespace WutheringWavesTool;

public static class Instance
{
    public static IServiceProvider? Service { get; private set; }

    public static void InitService()
    {
        Service = new ServiceCollection()
            #region View and ViewModel
            .AddSingleton<ShellPage>()
            .AddSingleton<ShellViewModel>()
            .AddTransient<MainGameViewModel>()
            .AddTransient<BilibiliGameViewModel>()
            .AddTransient<GlobalGameViewModel>()
            .AddTransient<SettingViewModel>()
            #endregion
            #region Navigation
            .AddTransient<IPageService, PageService>()
            .AddTransient<IPickersService, PickersService>()
            #endregion
            #region Base
            .AddSingleton<IAppContext<App>, AppContext<App>>()
            #endregion
            #region Navigation
            .AddKeyedSingleton<INavigationService, HomeNavigationService>(
                nameof(HomeNavigationService)
            )
            #endregion
            .AddGameContext()
            .BuildServiceProvider();
    }
}
