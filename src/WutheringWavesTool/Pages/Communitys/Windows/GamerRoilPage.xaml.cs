using WutheringWavesTool.Models.Wrapper.WindowRoils;

namespace WutheringWavesTool.Pages.Communitys.Windows;

public sealed partial class GamerRoilPage : Page, IPage
{
    public GamerRoilPage()
    {
        this.InitializeComponent();
        this.ViewModel = Instance.Service.GetRequiredService<GamerRoilViewModel>();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        this.ViewModel.SetData(e.Parameter as NavigationRoilsDetilyItem);
        base.OnNavigatedTo(e);
    }

    public GamerRoilViewModel ViewModel { get; }

    public Type PageType => typeof(GamerRoilPage);
}
