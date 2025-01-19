using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using WutheringWavesTool.Controls;
using WutheringWavesTool.Models;

namespace WutheringWavesTool.Services.Contracts;

public interface IWallpaperService
{
    public string NowHexValue { get; }
    public Task<bool> SetWrallpaper(string path);

    public void RegisterImageHost(ImageEx image);

    public void RegisterHostPath(string folder);

    IAsyncEnumerable<WallpaperModel> GetFilesAsync(CancellationToken token = default);
}
