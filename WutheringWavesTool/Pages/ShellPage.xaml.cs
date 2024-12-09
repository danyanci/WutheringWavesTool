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
    }

    public ShellViewModel ViewModel { get; }

    private void ShellPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        this.titlebar.UpDate();
    }

    private void appIcon_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) { }
}
