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

    private async void ShellPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        this.titlebar.UpDate();
        this.ViewModel.AppContext.RegisterRoot(this.XamlRoot);
        await this.ViewModel.WallpaperService.RegisterImageHostAsync(this.image);
        //this.player.MediaPlayer.IsLoopingEnabled = true;
    }
}
