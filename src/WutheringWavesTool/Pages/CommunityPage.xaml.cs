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
        if (dataSelect != null)
            dataSelect.SelectionChanged -= dataSelect_SelectionChanged;
        if (this.frame.Content is IDisposable disposable)
        {
            disposable.Dispose();
        }
        this.Dispose();
        GC.Collect();
        base.OnNavigatedFrom(e);
    }

    public Type PageType => typeof(CommunityPage);

    public CommunityViewModel ViewModel { get; private set; }

    private async void dataSelect_SelectionChanged(
        SelectorBar sender,
        SelectorBarSelectionChangedEventArgs args
    )
    {
        if (sender.SelectedItem.Tag == null)
            return;
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
                ViewModel.NavigationService.NavigationTo<GamerChallengeViewModel>(
                    this.ViewModel.SelectRoil,
                    new Microsoft.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo()
                );
                break;
            case "DataAbyss":
                ViewModel.NavigationService.NavigationTo<GamerTowerViewModel>(
                    this.ViewModel.SelectRoil,
                    new Microsoft.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo()
                );
                break;
            case "DataWorld":
                ViewModel.NavigationService.NavigationTo<GamerExploreIndexViewModel>(
                    this.ViewModel.SelectRoil,
                    new Microsoft.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo()
                );
                break;
        }
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                this.Bindings.StopTracking();
                this.ViewModel.NavigationService.UnRegisterView();
                this.ViewModel.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
