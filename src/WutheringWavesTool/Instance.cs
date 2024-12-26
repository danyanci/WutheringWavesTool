using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Waves.Core;
using WavesLauncher.Core.Contracts;
using WutheringWavesTool.Pages;
using WutheringWavesTool.Pages.Communitys;
using WutheringWavesTool.Pages.Dialogs;
using WutheringWavesTool.Services;
using WutheringWavesTool.Services.Contracts;
using WutheringWavesTool.Services.Navigations;
using WutheringWavesTool.ViewModel;
using WutheringWavesTool.ViewModel.Communitys;
using WutheringWavesTool.ViewModel.DialogViewModels;
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
            .AddTransient<IViewFactorys, ViewFactorys>()
            .AddSingleton<ShellViewModel>()
            .AddTransient<MainGameViewModel>()
            .AddTransient<BilibiliGameViewModel>()
            .AddTransient<GlobalGameViewModel>()
            .AddTransient<SettingViewModel>()
            .AddTransient<CommunityViewModel>()
            #region Community
            .AddTransient<GamerSignPage>()
            .AddTransient<GamerSignViewModel>()
            .AddTransient<GameRoilsViewModel>()
            .AddTransient<GamerDockViewModel>()
            .AddTransient<GamerChallengeViewModel>()
            #endregion
            #region Dialog
            .AddTransient<LoginDialog>()
            .AddTransient<LoginViewModel>()
            .AddTransient<BindGameDataDialog>()
            .AddTransient<BindGameDataViewModel>()
            #endregion
            #endregion
            #region Navigation
            .AddTransient<IPageService, PageService>()
            .AddTransient<IPickersService, PickersService>()
            .AddSingleton<ITipShow, TipShow>()
            #endregion
            #region Base
            .AddSingleton<IAppContext<App>, AppContext<App>>()
            .AddSingleton<IWavesClient, WavesClient>()
            #endregion
            #region Navigation
            .AddKeyedSingleton<INavigationService, HomeNavigationService>(
                nameof(HomeNavigationService)
            )
            .AddKeyedTransient<INavigationService, CommunityNavigationService>(
                nameof(CommunityNavigationService)
            )
            #endregion
            .AddGameContext()
            .BuildServiceProvider();
    }
}
