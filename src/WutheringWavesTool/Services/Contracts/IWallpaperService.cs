namespace WutheringWavesTool.Services.Contracts;

public interface IWallpaperService
{
    public string NowHexValue { get; }
    public Task<bool> SetWrallpaper(string path);

    public Task RegisterImageHostAsync(ImageEx image);

    public void RegisterHostPath(string folder);

    IAsyncEnumerable<WallpaperModel> GetFilesAsync(CancellationToken token = default);
}
