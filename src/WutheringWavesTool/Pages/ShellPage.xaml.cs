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
        Loaded += ShellPage_Loaded1;
    }

    private void ShellPage_Loaded1(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        this.ViewModel.AppContext.RegisterRoot(this.XamlRoot);
    }

    public ShellViewModel ViewModel { get; }

    private void ShellPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        this.titlebar.UpDate();
    }

    private void appIcon_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) { }
}
