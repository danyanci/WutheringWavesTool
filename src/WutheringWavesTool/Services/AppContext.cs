using Waves.Core.Services;
using WavesLauncher.Core.Contracts;

namespace WutheringWavesTool.Services;

public class AppContext<T> : IAppContext<T>
    where T : ClientApplication
{
    public AppContext(IWavesClient wavesClient)
    {
        WavesClient = wavesClient;
    }

    private ContentDialog _dialog;

    public T App { get; private set; }

    public IWavesClient WavesClient { get; }

    public async Task LauncherAsync(T app)
    {
        await Instance
            .Service!.GetRequiredKeyedService<IGameContext>(nameof(MainGameContext))
            .InitAsync();
        await Instance
            .Service!.GetRequiredKeyedService<IGameContext>(nameof(BilibiliGameContext))
            .InitAsync();
        await Instance
            .Service!.GetRequiredKeyedService<IGameContext>(nameof(GlobalGameContext))
            .InitAsync();
        this.App = app;
        var win = new WinUIEx.WindowEx();
        var page = Instance.Service!.GetRequiredService<ShellPage>();
        page.titlebar.Window = win;
        win.Content = page;
        win.SystemBackdrop = new MicaBackdrop()
        {
            Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt,
        };
        var winManager = WindowManager.Get(win);
        winManager.MaxWidth = 1090;
        winManager.MaxHeight = 670;
        winManager.IsResizable = false;
        winManager.IsMaximizable = false;
        this.App.MainWindow = win;
        win.Activate();

        if (win.Content is FrameworkElement fe)
        {
            fe.RequestedTheme =
                AppSettings.AppTheme == null ? ElementTheme.Default
                : AppSettings.AppTheme == "Dark" ? ElementTheme.Dark
                : AppSettings.AppTheme == "Light" ? ElementTheme.Light
                : ElementTheme.Default;
        }
        if (await WavesClient.IsLoginAsync())
        {
            var gamers = await WavesClient.GetWavesGamerAsync();
            if (gamers != null && gamers.Success)
            {
                foreach (var item in gamers.Data)
                {
                    var data = await WavesClient.RefreshGamerDataAsync(item);
                }
            }
        }
        this.App.MainWindow.AppWindow.Closing += AppWindow_Closing;
    }

    private void AppWindow_Closing(
        Microsoft.UI.Windowing.AppWindow sender,
        Microsoft.UI.Windowing.AppWindowClosingEventArgs args
    )
    {
        args.Cancel = true;
        var mainContext = Instance.Service!.GetRequiredKeyedService<IGameContext>(
            nameof(MainGameContext)
        );
        //mainContext.CancelDownloadAsync().GetAwaiter().GetResult();

        var biliContext = Instance.Service!.GetRequiredKeyedService<IGameContext>(
            nameof(BilibiliGameContext)
        );
        // biliContext.CancelDownloadAsync().GetAwaiter().GetResult();
        var globalContext = Instance.Service!.GetRequiredKeyedService<IGameContext>(
            nameof(GlobalGameContext)
        );
        //globalContext.CancelDownloadAsync().GetAwaiter().GetResult();
        Process.GetCurrentProcess().Kill();
    }

    public async Task TryInvokeAsync(Action action)
    {
        await CommunityToolkit.WinUI.DispatcherQueueExtensions.EnqueueAsync(
            this.App.MainWindow.DispatcherQueue,
            action,
            priority: Microsoft.UI.Dispatching.DispatcherQueuePriority.High
        );
    }

    public ElementTheme CurrentElement { get; set; }

    public void SetElementTheme(ElementTheme theme)
    {
        if (this.App.MainWindow.Content is FrameworkElement fe)
        {
            fe.RequestedTheme = theme;
            this.CurrentElement = theme;
        }
    }
}
