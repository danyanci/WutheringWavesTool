namespace WutheringWaves.Core.Contracts;

public interface IHttpClientService
{
    public HttpClient HttpClient { get; }

    public HttpClient GameDownloadClient { get; }

    public void BuildClient();
}
