using WutheringWavesTool.Pages.Bases;

namespace WutheringWavesTool.Pages.GamePages;

public sealed partial class BilibiliGamePage : GamePageBase, IPage
{
    public BilibiliGamePage()
    {
        this.InitializeComponent();
        this.ViewModel = Instance.Service?.GetRequiredService<BilibiliGameViewModel>();
    }

    public Type PageType => typeof(BilibiliGamePage);

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

    public BilibiliGameViewModel? ViewModel { get; protected set; }
}
