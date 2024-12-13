using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Media;
using WutheringWavesTool.Services.Contracts;
using WutheringWavesTool.WindowModels;

namespace WutheringWavesTool.Services;

public class WindowFactorys : IWindowFactorys
{
    public GetGeetWindow CreateGeetWindow()
    {
        var windw = new GetGeetWindow();
        windw.MaxHeight = 510;
        windw.MaxWidth = 700;
        return windw;
    }
}
