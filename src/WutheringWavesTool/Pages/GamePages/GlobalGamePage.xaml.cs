namespace WutheringWavesTool.Pages.GamePages;

public sealed partial class GlobalGamePage : Page, IPage
{
    public GlobalGamePage()
    {
        this.InitializeComponent();
    }

    public Type PageType => typeof(GlobalGamePage);
}
