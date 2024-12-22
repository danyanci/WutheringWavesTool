using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WutheringWavesTool.Common;
using WutheringWavesTool.ViewModel;
using WutheringWavesTool.ViewModel.Communitys;

namespace WutheringWavesTool.Pages;

public sealed partial class CommunityPage : Page, IPage, IDisposable
{
    private bool disposedValue;

    public CommunityPage()
    {
        this.InitializeComponent();
        this.ViewModel = Instance.Service.GetRequiredService<CommunityViewModel>();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        this.Dispose();
    }

    public Type PageType => typeof(CommunityPage);

    public CommunityViewModel ViewModel { get; private set; }

    private async void dataSelect_SelectionChanged(
        SelectorBar sender,
        SelectorBarSelectionChangedEventArgs args
    )
    {
        switch (sender.SelectedItem.Tag.ToString())
        {
            case "DataGamer":
                ViewModel.NavigationService.NavigationTo<GameRoilsViewModel>(
                    this.ViewModel.SelectRoil,
                    new Microsoft.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo()
                );
                break;
            case "DataDock":
                ViewModel.NavigationService.NavigationTo<GamerDockViewModel>(
                    this.ViewModel.SelectRoil,
                    new Microsoft.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo()
                );
                break;
            case "DataChallenge":
                break;
            case "DataAbyss":
                break;
            case "DataWorld":
                break;
        }
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                this.ViewModel.NavigationService.UnRegisterView();
                this.Bindings.StopTracking();
                this.ViewModel.Dispose();
                this.ViewModel = null;
                GC.Collect();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
    }
}
