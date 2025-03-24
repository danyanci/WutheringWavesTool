using WutheringWaves.Core.Common;
using WutheringWaves.Core.Contracts;

namespace WutheringWaves.Core.Services;

public class HttpClientService : IHttpClientService
{
    public HttpClientService(IHttpClientFactory httpClientFactory)
    {
        HttpClientFactory = httpClientFactory;
    }

    public HttpClient HttpClient { get; private set; }
    public IHttpClientFactory HttpClientFactory { get; }

    public HttpClient GameDownloadClient { get; private set; }

    public void BuildClient()
    {
        HttpClient = new HttpClient(new WavesGameHandler());
        GameDownloadClient = new HttpClient(new WavesGameHandler());
    }
}
