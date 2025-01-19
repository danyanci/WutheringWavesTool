using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using WutheringWavesTool.Controls;
using WutheringWavesTool.Helpers;
using WutheringWavesTool.Models;
using WutheringWavesTool.Services.Contracts;

namespace WutheringWavesTool.Services;

public class WallpaperService : IWallpaperService
{
    public WallpaperService(ITipShow tipShow)
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

    public async IAsyncEnumerable<WallpaperModel> GetFilesAsync(
        [EnumeratorCancellation] CancellationToken token = default
    )
    {
        List<WallpaperModel> models = new();
        var folder = new DirectoryInfo(this.BaseFolder);
        using (MD5 md5 = MD5.Create())
        {
            var files = Directory
                .GetFiles(this.BaseFolder, "*.*", SearchOption.TopDirectoryOnly)
                .Where(s =>
                    s.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                    || s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                );
            foreach (var item in files)
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
                using (
                    var stream = new FileStream(
                        item,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read,
                        bufferSize: 4096,
                        useAsync: true
                    )
                )
                {
                    byte[] hashBytes = await md5.ComputeHashAsync(stream);
                    var md5Value = BitConverter
                        .ToString(hashBytes)
                        .Replace("-", "")
                        .ToLowerInvariant();
                    yield return new()
                    {
                        FilePath = item,
                        Image = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new(item)),
                        Md5String = md5Value,
                    };
                }
            }
        }
    }
}
