using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Waves.Api.Models.Communitys;
using WutheringWavesTool.Common;
using WutheringWavesTool.ViewModel.Communitys;

namespace WutheringWavesTool.Pages.Communitys;

public sealed partial class GameCountPage : Page, IPage
{
    public GameCountPage()
    {
        this.InitializeComponent();
        this.ViewModel = Instance.Service!.GetRequiredService<GameCountViewModel>();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is GameRoilDataItem item)
        {
            await this.ViewModel.SetRoilAsync(item);
        }
        base.OnNavigatedTo(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        this.ViewModel.Dispose();
        base.OnNavigatedFrom(e);
    }

    public Type PageType => typeof(GameCountPage);

    public GameCountViewModel ViewModel { get; }
}
