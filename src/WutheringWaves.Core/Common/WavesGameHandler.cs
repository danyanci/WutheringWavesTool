using System.Net;

namespace WutheringWaves.Core.Common;

public class WavesGameHandler : HttpClientHandler
{
    public WavesGameHandler()
    {
        AutomaticDecompression = DecompressionMethods.All;
    }
}
