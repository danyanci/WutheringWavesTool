using System.Threading.Tasks;
using WutheringWavesTool.Controls;
using WutheringWavesTool.Helpers;
using WutheringWavesTool.Services.Contracts;

namespace WutheringWavesTool.Services;

public class WrallpaperService : IWrallpaperService
{
    public WrallpaperService(ITipShow tipShow)
    {
        TipShow = tipShow;
    }

    public string BaseFolder { get; private set; }
    public ImageEx ImageHost { get; private set; }
    public ITipShow TipShow { get; }
    public string NowHexValue { get; private set; }

    public void RegisterHostPath(string folder)
    {
        this.BaseFolder = folder;
    }

    public void RegisterImageHost(ImageEx image)
    {
        this.ImageHost = image;
    }

    public async Task<bool> SetWrallpaper(string path)
    {
        var result = await ImageIOHelper.HexImageAsync(this.BaseFolder, path);
        if (result.Item1 != null)
        {
            this.ImageHost.Source = result.Item1;
            if (TipShow != null)
                TipShow.ShowMessage(result.Item2, Microsoft.UI.Xaml.Controls.Symbol.Pictures);
            this.NowHexValue = result.Item3!;
            return true;
        }
        else
        {
            if (TipShow != null)
                TipShow.ShowMessage(result.Item2, Microsoft.UI.Xaml.Controls.Symbol.Pictures);
            return false;
        }
    }
}
