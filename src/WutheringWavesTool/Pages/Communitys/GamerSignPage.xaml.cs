using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Waves.Api.Models.Communitys;
using WutheringWavesTool.Common;
using WutheringWavesTool.ViewModel.Communitys;

namespace WutheringWavesTool.Pages.Communitys;

public sealed partial class GamerSignPage : Page, IWindowPage
{
    private Window window;

    public GamerSignPage()
    {
        this.InitializeComponent();
        this.ViewModel = Instance.Service!.GetRequiredService<GamerSignViewModel>();
    }

    public GamerSignViewModel ViewModel { get; }

    public void SetData(object value)
    {
        if (value is GameRoilDataItem item)
        {
            this.ViewModel.SignRoil = item;
        }
    }

    public void SetWindow(Window window)
    {
        this.titlebar.Window = window;
        this.titlebar.IsExtendsContentIntoTitleBar = true;
        this.titlebar.UpDate();
    }
}
