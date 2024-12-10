using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Navigation;
using WutheringWavesTool.Common;
using WutheringWavesTool.Pages.Bases;
using WutheringWavesTool.ViewModel.GameViewModels;

namespace WutheringWavesTool.Pages.GamePages;

public sealed partial class MainGamePage : GamePageBase, IPage
{
    public MainGamePage()
    {
        this.InitializeComponent();
        this.ViewModel = Instance.Service?.GetRequiredService<MainGameViewModel>();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (this.ViewModel != null)
            this.ViewModel.Dispose();
        this.ViewModel = null;
        GC.Collect();
        base.OnNavigatedFrom(e);
    }

    public Type PageType => typeof(MainGamePage);

    public MainGameViewModel? ViewModel { get; protected set; }
}
