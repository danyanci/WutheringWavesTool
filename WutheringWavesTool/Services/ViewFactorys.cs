using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Waves.Api.Models.Communitys;
using WutheringWavesTool.Common;
using WutheringWavesTool.Common.Bases;
using WutheringWavesTool.Pages.Communitys;
using WutheringWavesTool.Services.Contracts;
using WutheringWavesTool.WindowModels;

namespace WutheringWavesTool.Services;

public class ViewFactorys : IViewFactorys
{
    public ViewFactorys(IAppContext<App> appContext)
    {
        AppContext = appContext;
    }

    public IAppContext<App> AppContext { get; }

    public GetGeetWindow CreateGeetWindow()
    {
        var windw = new GetGeetWindow();
        windw.MaxHeight = 510;
        windw.MaxWidth = 700;
        return windw;
    }

    public WindowModelBase ShowSignWindow(GameRoilDataItem role) =>
        this.ShowWindowBase<GamerSignPage>(role);

    public WindowModelBase ShowWindowBase<T>(object data)
        where T : UIElement, IWindowPage
    {
        var win = new WindowModelBase();
        var page = Instance.Service!.GetRequiredService<T>();
        page.SetData(data);
        page.SetWindow(win);
        win.IsTitleBarVisible = true;
        win.Content = page;
        if (win.Content is FrameworkElement fs)
        {
            fs.RequestedTheme = ElementTheme.Dark;
        }
        return win;
    }
}
