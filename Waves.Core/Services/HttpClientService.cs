using Waves.Core.Contracts;

namespace Waves.Core.Services;

public class HttpClientService : IHttpClientService
{
    public HttpClientService(IHttpClientFactory httpClientFactory)
    {
        HttpClientFactory = httpClientFactory;
    }

    public HttpClient HttpClient { get; private set; }
    public IHttpClientFactory HttpClientFactory { get; }

    public HttpClient GameDownloadClient { get; private set; }

    public void BuildClient(string name)
    {
        this.HttpClient = HttpClientFactory.CreateClient(name);
        this.GameDownloadClient = HttpClientFactory.CreateClient(name + "_GameDownload");
    }
}
