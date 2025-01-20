namespace WutheringWavesTool.Pages.Communitys.Windows;

public sealed partial class GamerRoilDetilyPage : Page, IWindowPage
{
    public GamerRoilDetilyPage()
    {
        this.InitializeComponent();
        this.ViewModel = Instance.Service!.GetRequiredService<GamerRoilDetilyViewModel>();
    }

    public GamerRoilDetilyViewModel ViewModel { get; }

    public void SetData(object value)
    {
        if (value is GamerRoilDetily data)
        {
            this.ViewModel.SetData(data);
        }
    }

    public void SetWindow(Window window)
    {
        this.titlebar.Window = window;
        titlebar.UpDate();
    }
}
