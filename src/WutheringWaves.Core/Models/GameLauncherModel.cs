using System.Text.Json.Serialization;

namespace WutheringWaves.Core.Models;

public class CdnList
{
    [JsonPropertyName("P")]
    public int P { get; set; }

    [JsonPropertyName("K1")]
    public int K1 { get; set; }

    [JsonPropertyName("K2")]
    public int K2 { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}

public class Changelog { }

public class Config
{
    [JsonPropertyName("indexFileMd5")]
    public string IndexFileMd5 { get; set; }

    [JsonPropertyName("unCompressSize")]
    public long UnCompressSize { get; set; }

    [JsonPropertyName("baseUrl")]
    public string BaseUrl { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }

    [JsonPropertyName("patchType")]
    public string PatchType { get; set; }

    [JsonPropertyName("indexFile")]
    public string IndexFile { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("patchConfig")]
    public List<PatchConfig> PatchConfig { get; set; }
}

public class CurrentGameInfo
{
    [JsonPropertyName("fileName")]
    public string FileName { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("md5")]
    public string Md5 { get; set; }
}

public class ResourceDefault
{
    [JsonPropertyName("sampleHashSwitch")]
    public int SampleHashSwitch { get; set; }

    [JsonPropertyName("cdnList")]
    public List<CdnList> CdnList { get; set; }

    [JsonPropertyName("resourcesBasePath")]
    public string ResourcesBasePath { get; set; }

    [JsonPropertyName("changelog")]
    public Changelog Changelog { get; set; }

    [JsonPropertyName("resources")]
    public string Resources { get; set; }

    [JsonPropertyName("resourcesExcludePathNeedUpdate")]
    public List<object> ResourcesExcludePathNeedUpdate { get; set; }

    [JsonPropertyName("config")]
    public Config Config { get; set; }

    [JsonPropertyName("resourcesDiff")]
    public ResourcesDiff ResourcesDiff { get; set; }

    [JsonPropertyName("resourcesExcludePath")]
    public List<object> ResourcesExcludePath { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("changelogVisible")]
    public int ChangelogVisible { get; set; }
}

public class Download
{
    [JsonPropertyName("downloadReadBlockTimeout")]
    public string DownloadReadBlockTimeout { get; set; }
}

public class Experiment
{
    [JsonPropertyName("download")]
    public Download Download { get; set; }

    [JsonPropertyName("res_check")]
    public ResCheck ResCheck { get; set; }
}

public class Ext
{
    [JsonPropertyName("maxFileSize")]
    public object MaxFileSize { get; set; }
}

public class PatchConfig
{
    [JsonPropertyName("indexFileMd5")]
    public string IndexFileMd5 { get; set; }

    [JsonPropertyName("unCompressSize")]
    public object UnCompressSize { get; set; }

    [JsonPropertyName("baseUrl")]
    public string BaseUrl { get; set; }

    [JsonPropertyName("size")]
    public object Size { get; set; }

    [JsonPropertyName("indexFile")]
    public string IndexFile { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("ext")]
    public Ext Ext { get; set; }
}

public class Predownload
{
    [JsonPropertyName("changelog")]
    public Changelog Changelog { get; set; }

    [JsonPropertyName("config")]
    public Config Config { get; set; }

    [JsonPropertyName("resources")]
    public string Resources { get; set; }

    [JsonPropertyName("resourcesBasePath")]
    public string ResourcesBasePath { get; set; }

    [JsonPropertyName("resourcesDiff")]
    public ResourcesDiff ResourcesDiff { get; set; }

    [JsonPropertyName("resourcesExcludePath")]
    public List<object> ResourcesExcludePath { get; set; }

    [JsonPropertyName("resourcesExcludePathNeedUpdate")]
    public List<object> ResourcesExcludePathNeedUpdate { get; set; }

    [JsonPropertyName("sampleHashSwitch")]
    public int SampleHashSwitch { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }
}

public class PreviousGameInfo
{
    [JsonPropertyName("fileName")]
    public string FileName { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("md5")]
    public string Md5 { get; set; }
}

public class ResCheck
{
    [JsonPropertyName("fileChunkCheckSwitch")]
    public string FileChunkCheckSwitch { get; set; }

    [JsonPropertyName("fileSizeCheckSwitch")]
    public string FileSizeCheckSwitch { get; set; }

    [JsonPropertyName("resValidCheckTimeOut")]
    public string ResValidCheckTimeOut { get; set; }

    [JsonPropertyName("fileCheckWhiteListConfig")]
    public string FileCheckWhiteListConfig { get; set; }
}

public class ResourcesDiff
{
    [JsonPropertyName("currentGameInfo")]
    public CurrentGameInfo CurrentGameInfo { get; set; }

    [JsonPropertyName("previousGameInfo")]
    public PreviousGameInfo PreviousGameInfo { get; set; }
}

public class ResourcesLogin
{
    [JsonPropertyName("host")]
    public string Host { get; set; }

    [JsonPropertyName("loginSwitch")]
    public int LoginSwitch { get; set; }
}

public class RHIOptionList
{
    [JsonPropertyName("cmdOption")]
    public string CmdOption { get; set; }

    [JsonPropertyName("text")]
    public Text Text { get; set; }

    [JsonPropertyName("isShow")]
    public int IsShow { get; set; }
}

public class GameLauncherModel
{
    [JsonPropertyName("chunkDownloadSwitch")]
    public int ChunkDownloadSwitch { get; set; }

    [JsonPropertyName("keyFileCheckSwitch")]
    public int KeyFileCheckSwitch { get; set; }

    [JsonPropertyName("resourcesLogin")]
    public ResourcesLogin ResourcesLogin { get; set; }

    [JsonPropertyName("checkExeIsRunning")]
    public int CheckExeIsRunning { get; set; }

    [JsonPropertyName("hashCacheCheckAccSwitch")]
    public int HashCacheCheckAccSwitch { get; set; }

    [JsonPropertyName("fingerprints")]
    public List<string> Fingerprints { get; set; }

    [JsonPropertyName("default")]
    public ResourceDefault ResourceDefault { get; set; }

    [JsonPropertyName("RHIOptionSwitch")]
    public int RHIOptionSwitch { get; set; }

    [JsonPropertyName("predownloadSwitch")]
    public int PredownloadSwitch { get; set; }

    [JsonPropertyName("RHIOptionList")]
    public List<RHIOptionList> RHIOptionList { get; set; }

    [JsonPropertyName("experiment")]
    public Experiment Experiment { get; set; }

    [JsonPropertyName("predownload")]
    public Predownload Predownload { get; set; }

    [JsonPropertyName("keyFileCheckList")]
    public List<string> KeyFileCheckList { get; set; }
}

public class Text
{
    [JsonPropertyName("zh-Hans")]
    public string ZhHans { get; set; }

    [JsonPropertyName("de")]
    public string De { get; set; }

    [JsonPropertyName("zh-Hant")]
    public string ZhHant { get; set; }

    [JsonPropertyName("ko")]
    public string Ko { get; set; }

    [JsonPropertyName("ja")]
    public string Ja { get; set; }

    [JsonPropertyName("en")]
    public string En { get; set; }

    [JsonPropertyName("fr")]
    public string Fr { get; set; }

    [JsonPropertyName("es")]
    public string Es { get; set; }
}
