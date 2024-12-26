using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace WutheringWavesTool.Helpers
{
    public static class ImageIOHelper
    {
        public static async Task<BitmapImage> GetNetworkAsync(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var stream = await httpClient.GetStreamAsync(url);

                InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();

                using (var originalStream = stream.AsInputStream())
                {
                    await RandomAccessStream.CopyAsync(originalStream, randomAccessStream);
                }

                randomAccessStream.Seek(0);
                BitmapImage source = new BitmapImage();
                await source.SetSourceAsync(randomAccessStream);
                return source;
            }
        }

        public static async Task<BitmapImage> GetLocalFileAsync(string url)
        {
            BitmapImage source = new BitmapImage();
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(url));
            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                await source.SetSourceAsync(fileStream);
            }
            return source;
        }
    }
}
