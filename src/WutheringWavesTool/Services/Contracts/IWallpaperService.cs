using WutheringWavesTool.Helpers;

namespace WutheringWavesTool.Services.Contracts;

public interface IWallpaperService
{
    public string NowHexValue { get; }
    public Task<bool> SetWrallpaper(string path);

    public bool PletteEnable { get; set; }
    public event WallpaperPletteChangedDelegate WallpaperPletteChanged;
    public Task RegisterImageHostAsync(Controls.ImageEx image);

    public ImageColorPaletteHelper ColorPlette { get; }

    public void RegisterHostPath(string folder);

    IAsyncEnumerable<WallpaperModel> GetFilesAsync(CancellationToken token = default);
}
