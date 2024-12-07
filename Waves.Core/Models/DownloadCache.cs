using System.Text.Json.Serialization;
using Waves.Api.Models;

namespace Waves.Core.Models;

[JsonSerializable(typeof(DownloadCache))]
[JsonSerializable(typeof(Resource))]
[JsonSerializable(typeof(DownloadLocalResource))]
[JsonSerializable(typeof(List<Resource>))]
public sealed partial class DownloadCacheJsonContext : JsonSerializerContext { }

public class DownloadCache
{
    [JsonPropertyName("isComplete")]
    public bool IsComplete { get; internal set; } = false;

    [JsonPropertyName("progress")]
    public double Progress { get; set; }

    [JsonPropertyName("totalSize")]
    public double TotalSize { get; set; }

    [JsonPropertyName("nowSize")]
    public double NowSize { get; set; }

    [JsonPropertyName("currentDownloadFile")]
    public string CurrentDownloadFile { get; set; }

    [JsonPropertyName("currentDownloadProgress")]
    public long CurrentDownloadFileSize { get; set; }

    [JsonPropertyName("downloadResources")]
    public DownloadLocalResource DownloadResource { get; set; }
}

public class DownloadLocalResource
{
    [JsonPropertyName("resources")]
    public List<Resource> Resources { get; set; }
}
