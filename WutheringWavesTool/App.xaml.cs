using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Waves.Core.GameContext;
using WutheringWavesTool.Common;
using WutheringWavesTool.Services.Contracts;

namespace WutheringWavesTool;

public partial class App : ClientApplication
{
    public App()
    {
        GameContextFactory.GameBassPath = "C:\\Users\\30140\\Documents\\Waves";
        Instance.InitService();
        this.InitializeComponent();
    }

    protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        await Instance.Service.GetRequiredService<IAppContext<App>>().LauncherAsync(this);
    }
}
