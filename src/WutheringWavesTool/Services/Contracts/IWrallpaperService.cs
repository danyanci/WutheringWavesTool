using System.Threading.Tasks;
using WutheringWavesTool.Controls;

namespace WutheringWavesTool.Services.Contracts;

public interface IWrallpaperService
{
    public string NowHexValue { get; }
    public Task<bool> SetWrallpaper(string path);

    public void RegisterImageHost(ImageEx image);

    public void RegisterHostPath(string folder);
}
