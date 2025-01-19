using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using Waves.Core.GameContext;
using WutheringWavesTool.Common;
using WutheringWavesTool.Services.Contracts;

namespace WutheringWavesTool;

public partial class App : ClientApplication
{
    public static string BassFolder =>
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Waves";

    public static string RecordFolder => BassFolder + "\\RecordCache";

    public static string WrallpaperFolder => BassFolder + "\\WallpaperImages";

    public App()
    {
        StaticConfig.EnableAot = true;
        Directory.CreateDirectory(BassFolder);
        Directory.CreateDirectory(RecordFolder);
        Directory.CreateDirectory(WrallpaperFolder);
        GameContextFactory.GameBassPath = BassFolder;
        Instance.InitService();

        this.UnhandledException += App_UnhandledException;
        this.InitializeComponent();
    }

    private void App_UnhandledException(
        object sender,
        Microsoft.UI.Xaml.UnhandledExceptionEventArgs e
    )
    {
        using (var fs = new StreamWriter("D:\\Test.txt"))
        {
            fs.WriteLine(e.Exception.Message);
        }
    }

    protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        await Instance.Service.GetRequiredService<IAppContext<App>>().LauncherAsync(this);
    }
}
