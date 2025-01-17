using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Waves.Core;
using WavesLauncher.Core.Contracts;
using WutheringWavesTool.Pages;
using WutheringWavesTool.Pages.Communitys;
using WutheringWavesTool.Pages.Communitys.Windows;
using WutheringWavesTool.Pages.Dialogs;
using WutheringWavesTool.Pages.Record;
using WutheringWavesTool.Services;
using WutheringWavesTool.Services.Contracts;
using WutheringWavesTool.Services.Navigations;
using WutheringWavesTool.ViewModel;
using WutheringWavesTool.ViewModel.Communitys;
using WutheringWavesTool.ViewModel.Communitys.WinViewModel;
using WutheringWavesTool.ViewModel.DialogViewModels;
using WutheringWavesTool.ViewModel.GameViewModels;
using WutheringWavesTool.ViewModel.Record;

namespace WutheringWavesTool;

public static class Instance
{
    public static IServiceProvider Service { get; private set; }

    public static void InitService()
    {
        Service = new ServiceCollection()
            #region View and ViewModel
            .AddSingleton<ShellPage>()
            .AddSingleton<ShellViewModel>()
            .AddTransient<PlayerRecordPage>()
            .AddTransient<PlayerRecordViewModel>()
            .AddTransient<MainGameViewModel>()
            .AddTransient<BilibiliGameViewModel>()
            .AddTransient<GlobalGameViewModel>()
            .AddTransient<SettingViewModel>()
            .AddTransient<CommunityViewModel>()
            #region Community
            .AddTransient<GamerSignPage>()
            .AddTransient<GamerSignViewModel>()
            .AddSingleton<GamerRoilDetilyPage>()
            .AddSingleton<GamerRoilDetilyViewModel>()
            .AddTransient<GameRoilsViewModel>()
            .AddTransient<GamerDockViewModel>()
            .AddTransient<GamerChallengeViewModel>()
            .AddTransient<GamerExploreIndexViewModel>()
            .AddTransient<GamerTowerViewModel>()
            .AddTransient<GamerSkinViewModel>()
            #endregion
            #region Record
            .AddTransient<RecordItemViewModel>()
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
            .AddTransient<IViewFactorys, ViewFactorys>()
            #endregion
            #region Navigation
            .AddKeyedSingleton<INavigationService, HomeNavigationService>(
                nameof(HomeNavigationService)
            )
            .AddKeyedTransient<INavigationService, CommunityNavigationService>(
                nameof(CommunityNavigationService)
            )
            #endregion
            #region Record
            .AddScoped<IDialogManager, DialogManager>()
            .AddScoped<ITipShow, TipShow>()
            .AddKeyedScoped<IPlayerRecordContext, PlayerRecordContext>("PlayerRecord")
            .AddKeyedScoped<INavigationService, RecordNavigationService>(
                nameof(RecordNavigationService)
            )
            .AddScoped<IRecordCacheService, RecordCacheService>()
            #endregion
            .AddGameContext()
            .BuildServiceProvider();
    }
}
