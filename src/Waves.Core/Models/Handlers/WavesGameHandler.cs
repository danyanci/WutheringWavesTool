using System.Net;

namespace Waves.Core.Models.Handlers;

public class WavesGameHandler : HttpClientHandler
{
    public WavesGameHandler()
    {
        AutomaticDecompression = DecompressionMethods.All;
    }
}
