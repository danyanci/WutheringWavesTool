using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Waves.Api.Models.Communitys;
using WutheringWavesTool.Common;
using WutheringWavesTool.ViewModel.Communitys;

namespace WutheringWavesTool.Pages.Communitys;

public sealed partial class GamerDockPage : UserControl, ICommunityViewModel
{
    public GamerDockPage()
    {
        this.InitializeComponent();
    }

    public GamerDockViewModel ViewModel { get; set; }

    public Type PageType => typeof(GamerDockPage);

    public void Dispose()
    {
        this.ViewModel.Dispose();
        this.ViewModel = null;
        GC.Collect();
    }

    public async Task SetDataAsync(GameRoilDataItem item)
    {
        await this.ViewModel.SetDataAsync(item);
    }
}
