using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Waves.Api.Models.Communitys;
using WutheringWavesTool.Common;
using WutheringWavesTool.ViewModel.Communitys;

namespace WutheringWavesTool.Pages.Communitys;

public sealed partial class GamerRoilsPage : Page, IPage
{
    private GameRoilsViewModel viewModel;

    public GamerRoilsPage()
    {
        this.InitializeComponent();
        this.ViewModel = Instance.Service.GetRequiredService<GameRoilsViewModel>();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is GameRoilDataItem item)
        {
            await this.ViewModel.SetDataAsync(item);
        }
        base.OnNavigatedTo(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        this.ViewModel.Dispose();
        base.OnNavigatedFrom(e);
    }

    public Type PageType => typeof(GamerRoilsPage);

    public GameRoilsViewModel ViewModel
    {
        get => viewModel;
        set => viewModel = value;
    }
}
