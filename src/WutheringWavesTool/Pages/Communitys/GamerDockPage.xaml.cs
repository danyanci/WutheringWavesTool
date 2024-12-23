using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Waves.Api.Models.Communitys;
using WutheringWavesTool.Common;
using WutheringWavesTool.ViewModel.Communitys;

namespace WutheringWavesTool.Pages.Communitys;

public sealed partial class GamerDockPage : Page, IPage
{
    public GamerDockPage()
    {
        this.InitializeComponent();
        this.ViewModel = Instance.Service.GetRequiredService<GamerDockViewModel>();
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
        this.Bindings.StopTracking();
        this.ViewModel.Dispose();
        base.OnNavigatedFrom(e);
    }

    public GamerDockViewModel ViewModel { get; }

    public Type PageType => typeof(GamerDockViewModel);
}
