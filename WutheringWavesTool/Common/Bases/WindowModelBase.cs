using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Newtonsoft.Json.Linq;
using WinUICommunity;
using WinUIEx;
using WutheringWavesTool.Services.Contracts;

namespace WutheringWavesTool.Common.Bases;

public partial class WindowModelBase : WindowEx
{
    public AppWindow AppWindowApp;

    OverlappedPresenter? Overlapped => this.AppWindow.Presenter as OverlappedPresenter;

    public WindowModelBase()
    {
        this.SystemBackdrop = new WinUICommunity.AcrylicSystemBackdrop(
            Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicKind.Default
        );
        if (Overlapped != null)
        {
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            Microsoft.UI.WindowId windowId1 = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindowApp = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId1);
            Microsoft.UI.Windowing.OverlappedPresenter presenter = (
                AppWindowApp.Presenter as Microsoft.UI.Windowing.OverlappedPresenter
            )!;
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.IsResizable = false;
            IntPtr baseHwnd = WinRT.Interop.WindowNative.GetWindowHandle(
                Instance.Service.GetRequiredService<IAppContext<App>>().App.MainWindow
            );
            WindowExtension.SetWindowLong(hWnd, WindowExtension.GWL_HWNDPARENT, baseHwnd);
            presenter.IsModal = true;
            this.Closed += (s, e) =>
            {
                this.Content = null;
                GC.Collect();
                WindowExtension.EnableWindow(baseHwnd, true);
            };
        }
    }
}
