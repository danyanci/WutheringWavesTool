using System.Text.Json.Serialization;

namespace Waves.Core.Models.Downloader;

public class ChunkInfo
{
    [JsonPropertyName("start")]
    public long Start { get; set; }

    [JsonPropertyName("end")]
    public long End { get; set; }

    [JsonPropertyName("md5")]
    public string Md5 { get; set; }
}

public class Resource
{
    [JsonPropertyName("dest")]
    public string Dest { get; set; }

    [JsonPropertyName("md5")]
    public string Md5 { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }

    [JsonPropertyName("chunkInfos")]
    public List<ChunkInfo> ChunkInfos { get; set; }

    [JsonPropertyName("start")]
    public long? Start { get; set; }

    [JsonPropertyName("end")]
    public long? End { get; set; }
}

public class GameResource
{
    [JsonPropertyName("resource")]
    public List<Resource> Resource { get; set; }
}
