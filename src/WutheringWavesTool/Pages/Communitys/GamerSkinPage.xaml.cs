using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Waves.Api.Models.Communitys;
using WutheringWavesTool.Common;
using WutheringWavesTool.ViewModel.Communitys;

namespace WutheringWavesTool.Pages.Communitys;

public sealed partial class GamerSkinPage : Page, IPage
{
    public GamerSkinPage()
    {
        this.InitializeComponent();
        this.ViewModel = Instance.Service.GetRequiredService<GamerSkinViewModel>();
    }

    public GamerSkinViewModel ViewModel { get; }

    public Type PageType => typeof(GamerSkinPage);

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is GameRoilDataItem item)
        {
            this.ViewModel.SetData(item);
        }
        base.OnNavigatedTo(e);
    }
}
