using WutheringWavesTool.Pages.Bases;

namespace WutheringWavesTool.Pages.GamePages;

public sealed partial class MainGamePage : GamePageBase, IPage
{
    public Type PageType => typeof(MainGamePage);

    public MainGameViewModel ViewModel { get; private set; }

    public MainGamePage()
    {
        this.InitializeComponent();
        this.ViewModel = Instance.Service?.GetRequiredService<MainGameViewModel>();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (this.ViewModel != null)
            this.ViewModel.Dispose();
        this.ViewModel = null;
        GC.Collect();
        base.OnNavigatedFrom(e);
    }

    private void SelectorBar_SelectionChanged(
        Microsoft.UI.Xaml.Controls.SelectorBar sender,
        Microsoft.UI.Xaml.Controls.SelectorBarSelectionChangedEventArgs args
    )
    {
        this.ViewModel.SelectNews(sender.SelectedItem.Tag.ToString());
    }
}
