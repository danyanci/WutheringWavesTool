using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
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
    }

    public void Dispose()
    {
        this.ViewModel.Dispose();
        this.ViewModel = null;
    }

    public Type PageType => typeof(GamerRoilsPage);

    public GameRoilsViewModel ViewModel
    {
        get => viewModel;
        set => viewModel = value;
    }
}
