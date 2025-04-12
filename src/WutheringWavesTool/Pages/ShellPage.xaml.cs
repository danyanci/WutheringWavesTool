using CommunityToolkit.WinUI.Behaviors;
using WutheringWavesTool.Pages.GamePages;

namespace WutheringWavesTool.Pages;

public sealed partial class ShellPage : Page
{
    public ShellPage()
    {
        this.InitializeComponent();
        this.ViewModel = Instance.Service.GetRequiredService<ShellViewModel>();
        this.Loaded += ShellPage_Loaded;
        this.ViewModel.HomeNavigationService.Navigated += HomeNavigationService_Navigated;
        this.ViewModel.HomeNavigationService.RegisterView(this.frame);
        this.ViewModel.HomeNavigationViewService.Register(this.navigationView);
        this.ViewModel.TipShow.Owner = this.panel;
        this.ViewModel.Image = this.image;
        this.ViewModel.BackControl = this.backControl;
    }

    private void HomeNavigationService_Navigated(object sender, NavigationEventArgs e)
    {
        if (e.SourcePageType == typeof(MainGamePage))
        {
            To0.Start(image);
        }
        else
        {
            To8.Start(image);
        }
        if (ViewModel.HomeNavigationService.CanGoBack)
        {
            this.ViewModel.BackVisibility = Visibility.Visible;
        }
        else
        {
            this.ViewModel.BackVisibility = Visibility.Collapsed;
        }
        ViewModel.SetSelectItem(e.SourcePageType);
        this.ViewModel.HomeNavigationService.ClearHistory();
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    public ShellViewModel ViewModel { get; }

    private async void ShellPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        this.titlebar.UpDate();
        this.ViewModel.DialogManager.RegisterRoot(this.XamlRoot);
        await this.ViewModel.WallpaperService.RegisterImageHostAsync(this.image);
    }
}
