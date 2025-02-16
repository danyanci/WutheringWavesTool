using Windows.System;
using WutheringWavesTool.Models.Wrapper.WindowRoils;

namespace WutheringWavesTool.Pages.Communitys.Windows;

public sealed partial class GamerRoilsDetilyPage : Page, IWindowPage
{
    public GamerRoilsDetilyPage(GamerRoilsDetilyViewModel viewModel)
    {
        this.InitializeComponent();
        ViewModel = viewModel;
        this.ViewModel.GamerRoilContext.NavigationService.RegisterView(this.frame);
    }

    public GamerRoilsDetilyViewModel ViewModel { get; }

    public void Dispose()
    {
        this.ViewModel.Close();
    }

    public void SetData(object value)
    {
        if (value is ShowRoleData data)
        {
            this.ViewModel.Data = data;
            this.ViewModel.SelectCache = data.Id;
        }
    }

    public void SetWindow(Window window)
    {
        this.titlebar.Window = window;
        this.titlebar.Window.AppWindow.Closing += AppWindow_Closing;
        titlebar.UpDate();
    }

    private void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        this.ViewModel.Close();
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

    private void view_SelectionChanged(
        NavigationView sender,
        NavigationViewSelectionChangedEventArgs args
    )
    {
        if (args.SelectedItem != null && args.SelectedItem is NavigationRoilsDetilyItem item)
        {
            this.ViewModel.SwitchPage(item);
        }
    }
}
