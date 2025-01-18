using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using WutheringWavesTool.ViewModel;

namespace WutheringWavesTool.Pages;

public sealed partial class ShellPage : Page
{
    public ShellPage()
    {
        this.InitializeComponent();
        this.ViewModel = Instance.Service.GetRequiredService<ShellViewModel>();
        this.Loaded += ShellPage_Loaded;
        this.ViewModel.HomeNavigationService.RegisterView(this.frame);
        this.ViewModel.TipShow.Owner = this.panel;
        this.ViewModel.Image = this.image;
        this.ViewModel.BackControl = this.backControl;
    }

    public ShellViewModel ViewModel { get; }

    private void ShellPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        this.titlebar.UpDate();
        this.ViewModel.AppContext.RegisterRoot(this.XamlRoot);
        //this.player.MediaPlayer.IsLoopingEnabled = true;
    }
}
