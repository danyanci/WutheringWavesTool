using System;
using Microsoft.Extensions.DependencyInjection;
using WutheringWavesTool.Common;
using WutheringWavesTool.Pages.Bases;
using WutheringWavesTool.ViewModel.GameViewModels;

namespace WutheringWavesTool.Pages.GamePages;

public sealed partial class MainGamePage : GamePageBase, IPage
{
    public MainGamePage()
    {
        this.InitializeComponent();
        this.ViewModel = Instance.Service.GetRequiredService<MainGameViewModel>();
    }

    public Type PageType => typeof(MainGamePage);

    public MainGameViewModel ViewModel { get; }
}
