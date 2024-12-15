using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.WinUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Waves.Core.GameContext;
using Waves.Core.GameContext.Contexts;
using WavesLauncher.Core.Contracts;
using WinUIEx;
using WutheringWavesTool.Common;
using WutheringWavesTool.Pages;
using WutheringWavesTool.Pages.Dialogs;
using WutheringWavesTool.Services.Contracts;

namespace WutheringWavesTool.Services
{
    public class AppContext<T> : IAppContext<T>
        where T : ClientApplication
    {
        public AppContext(IWavesClient wavesClient)
        {
            WavesClient = wavesClient;
        }

        private ContentDialog _dialog;

        public T App { get; private set; }

        public XamlRoot Root { get; private set; }
        public IWavesClient WavesClient { get; }

        public void RegisterRoot(XamlRoot root)
        {
            this.Root = root;
        }

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
                fe.RequestedTheme = ElementTheme.Dark;
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
            mainContext.CancelDownloadAsync().GetAwaiter().GetResult();

            var biliContext = Instance.Service!.GetRequiredKeyedService<IGameContext>(
                nameof(BilibiliGameContext)
            );
            biliContext.CancelDownloadAsync().GetAwaiter().GetResult();
            var globalContext = Instance.Service!.GetRequiredKeyedService<IGameContext>(
                nameof(GlobalGameContext)
            );
            globalContext.CancelDownloadAsync().GetAwaiter().GetResult();
            Process.GetCurrentProcess().Kill();
        }

        public async Task TryInvokeAsync(Action action)
        {
            await DispatcherQueueExtensions.EnqueueAsync(
                this.App.MainWindow.DispatcherQueue,
                action
            );
        }

        public async Task ShowLoginDialogAsync() => await ShowDialogAsync<LoginDialog>();

        public async Task ShowDialogAsync<T>()
            where T : ContentDialog, IDialog
        {
            if (_dialog != null)
                return;
            var dialog = Instance.Service.GetRequiredService<T>();
            dialog.XamlRoot = this.Root;
            this._dialog = dialog;
            await _dialog.ShowAsync();
        }

        public void CloseDialog()
        {
            if (_dialog == null)
                return;
            _dialog.Hide();
            _dialog = null;
            GC.Collect();
        }
    }
}
