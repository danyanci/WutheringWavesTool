using System.IO;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using SqlSugar;
using Waves.Core.GameContext;
using WutheringWavesTool.Common;
using WutheringWavesTool.Services.Contracts;

namespace WutheringWavesTool;

public partial class App : ClientApplication
{
    public App()
    {
        StaticConfig.EnableAot = true;
        GameContextFactory.GameBassPath = "C:\\Users\\30140\\Documents\\Waves";
        Instance.InitService();
        this.InitializeComponent();
        this.UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var file = "D:\\Debug.txt";
        using (var fs = new FileStream(file, FileMode.Create, FileAccess.ReadWrite))
        {
            fs.Write(Encoding.UTF8.GetBytes(e.Message.ToString()), 0, e.Message.ToString().Length);
        }
    }

    protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        await Instance.Service.GetRequiredService<IAppContext<App>>().LauncherAsync(this);
    }
}
