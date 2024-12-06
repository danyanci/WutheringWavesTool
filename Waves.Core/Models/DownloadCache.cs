using System.Text.Json.Serialization;

namespace Waves.Core.Models;

[JsonSerializable(typeof(DownloadCache))]
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
}
