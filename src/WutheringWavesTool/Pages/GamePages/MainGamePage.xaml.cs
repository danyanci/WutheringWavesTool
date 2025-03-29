using WutheringWavesTool.ViewModel.GameViewModels;

namespace WutheringWavesTool.Pages.GamePages;

public sealed partial class MainGamePage : Page, IPage
{
    public MainGamePage()
    {
        this.InitializeComponent();
        this.ViewModel = Instance.Service.GetRequiredService<MainGameViewModel>();
    }

    public MainGameViewModel ViewModel { get; }

    public Type PageType => typeof(MainGamePage);
}
