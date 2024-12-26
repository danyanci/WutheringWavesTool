using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using WutheringWavesTool.Common;
using WutheringWavesTool.Pages;
using WutheringWavesTool.Pages.Bases;
using WutheringWavesTool.Pages.Communitys;
using WutheringWavesTool.Pages.GamePages;
using WutheringWavesTool.Services.Contracts;
using WutheringWavesTool.ViewModel;
using WutheringWavesTool.ViewModel.Communitys;
using WutheringWavesTool.ViewModel.GameViewModels;

namespace WutheringWavesTool.Services;

public sealed partial class PageService : IPageService
{
    private readonly Dictionary<string, Type> _pages;

    public PageService()
    {
        _pages = new();
        this.RegisterView<MainGamePage, MainGameViewModel>();
        this.RegisterView<BilibiliGamePage, BilibiliGameViewModel>();
        this.RegisterView<GlobalGamePage, GlobalGameViewModel>();
        this.RegisterView<SettingPage, SettingViewModel>();
        this.RegisterView<CommunityPage, CommunityViewModel>();

        this.RegisterView<GamerRoilsPage, GameRoilsViewModel>();
        this.RegisterView<GamerDockPage, GamerDockViewModel>();
        this.RegisterView<GamerChallengePage, GamerChallengeViewModel>();
    }

    public Type GetPage(string key)
    {
        _pages.TryGetValue(key, out var page);
        if (page is null)
        {
            return null;
        }
        return page;
    }

    public void RegisterView<View, ViewModel>()
        where View : Page
        where ViewModel : ObservableObject
    {
        var key = typeof(ViewModel).FullName;
        if (_pages.ContainsKey(key))
        {
            throw new ArgumentException("已注册ViewModel");
        }
        if (_pages.ContainsValue(typeof(View)))
        {
            throw new ArgumentException("已注册View");
        }
        _pages.Add(key: typeof(ViewModel).FullName, typeof(View));
    }
}
