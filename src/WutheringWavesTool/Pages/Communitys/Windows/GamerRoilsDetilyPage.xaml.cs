using Windows.System;

namespace WutheringWavesTool.Pages.Communitys.Windows;

public sealed partial class GamerRoilsDetilyPage : Page, IWindowPage
{
    public GamerRoilsDetilyPage(GamerRoilsDetilyViewModel viewModel)
    {
        this.InitializeComponent();
        ViewModel = viewModel;
    }

    public GamerRoilsDetilyViewModel ViewModel { get; }

    public void SetData(object value)
    {
        if (value is ShowRoleData data)
        {
            this.ViewModel.Data = data;
        }
    }

    public void SetWindow(Window window)
    {
        this.titlebar.Window = window;
        titlebar.UpDate();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        this.view.IsPaneOpen = !this.view.IsPaneOpen;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        titlebar.UpDate();
    }
}
