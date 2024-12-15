using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Media;
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
}
