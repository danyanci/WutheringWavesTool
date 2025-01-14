using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WutheringWavesTool.Common;
using WutheringWavesTool.ViewModel;

namespace WutheringWavesTool.Pages.Record;

public sealed partial class PlayerRecordPage : Page, IWindowPage
{
    public PlayerRecordPage()
    {
        this.InitializeComponent();
        this.ViewModel = Instance.Service.GetRequiredService<PlayerRecordViewModel>();
        this.Loaded += PlayerRecordPage_Loaded;
    }

    private void PlayerRecordPage_Loaded(object sender, RoutedEventArgs e)
    {
        this.ViewModel.PlayerRecordContext.DialogManager.SetRoot(this.XamlRoot);
        this.ViewModel.PlayerRecordContext.TipShow.Owner = this.grid;
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        this.Loaded -= PlayerRecordPage_Loaded;
        base.OnNavigatedFrom(e);
    }

    public Window Window { get; private set; }
    public PlayerRecordViewModel ViewModel { get; private set; }

    public void SetData(object value) { }

    public void SetWindow(Window window)
    {
        this.Window = window;
        Window.AppWindow.Closing += AppWindow_Closing;
        this.titleBar.Window = Window;
        this.titleBar.UpDate();
    }

    private void AppWindow_Closing(
        Microsoft.UI.Windowing.AppWindow sender,
        Microsoft.UI.Windowing.AppWindowClosingEventArgs args
    )
    {
        this.Loaded -= PlayerRecordPage_Loaded;
        ViewModel.Dispose();
    }
}
