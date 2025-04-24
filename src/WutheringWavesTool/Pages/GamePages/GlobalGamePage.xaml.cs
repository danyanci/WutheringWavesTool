using WutheringWavesTool.ViewModel.GameViewModels;

namespace WutheringWavesTool.Pages.GamePages;

public sealed partial class GlobalGamePage : Page, IPage
{
    public GlobalGamePage()
    {
        this.InitializeComponent();
        this.ViewModel = Instance.Service.GetRequiredService<GlobalGameViewModel>();
    }

    public Type PageType => typeof(GlobalGamePage);

    public GlobalGameViewModel ViewModel { get; }
}
