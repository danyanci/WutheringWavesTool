using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.WinUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using WinUIEx;
using WutheringWavesTool.Common;
using WutheringWavesTool.Pages;
using WutheringWavesTool.Services.Contracts;

namespace WutheringWavesTool.Services
{
    public class AppContext<T> : IAppContext<T>
        where T : ClientApplication
    {
        public T App { get; private set; }

        public Task LauncherAsync(T app)
        {
            this.App = app;
            var win = new WinUIEx.WindowEx();
            var page = Instance.Service.GetRequiredService<ShellPage>();
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
            //if (App.MainWindow.Content is FrameworkElement content)
            //{
            //    content.RequestedTheme = ElementTheme.Dark;
            //}
            win.Activate();
            return Task.CompletedTask;
        }

        public async Task TryInvokeAsync(Action action)
        {
            await DispatcherQueueExtensions.EnqueueAsync(
                this.App.MainWindow.DispatcherQueue,
                action
            );
        }
    }
}
