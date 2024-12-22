using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WutheringWavesTool.Common;
using WutheringWavesTool.ViewModel;
using WutheringWavesTool.ViewModel.Communitys;

namespace WutheringWavesTool.Pages;

public sealed partial class CommunityPage : Page, IPage
{
    public CommunityPage()
    {
        this.InitializeComponent();
        this.ViewModel = Instance.Service.GetRequiredService<CommunityViewModel>();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        this.ViewModel.Dispose();
        this.frame.Content = null;

        this.ViewModel = null;
        GC.Collect();
    }

    public Type PageType => typeof(CommunityPage);

    public CommunityViewModel ViewModel { get; private set; }

    private async void dataSelect_SelectionChanged(
        SelectorBar sender,
        SelectorBarSelectionChangedEventArgs args
    )
    {
        if (this.ViewModel.ChildViewModel != null)
        {
            this.ViewModel.ChildViewModel.Dispose();
            this.ViewModel.ChildViewModel = null;
            this.frame.Content = null;
            GC.Collect();
        }
        switch (sender.SelectedItem.Tag.ToString())
        {
            case "DataGamer":
                var data = Instance.Service.GetRequiredService<GameRoilsViewModel>();
                await data.SetDataAsync(this.ViewModel.SelectRoil);
                this.ViewModel.ChildViewModel = data;
                break;
            case "DataDock":
                var dock = Instance.Service.GetRequiredService<GamerDockViewModel>();
                await dock.SetDataAsync(this.ViewModel.SelectRoil);
                this.ViewModel.ChildViewModel = dock;
                break;
            case "DataChallenge":
                break;
            case "DataAbyss":
                break;
            case "DataWorld":
                break;
        }
    }
}
