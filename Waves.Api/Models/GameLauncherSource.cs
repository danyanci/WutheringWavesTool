using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Waves.Api.Models;

[JsonSerializable(typeof(GameLauncherSource))]
[JsonSerializable(typeof(AnimateBackground))]
[JsonSerializable(typeof(AnimateBackground))]
[JsonSerializable(typeof(AnimateBackgroundLanguage))]
[JsonSerializable(typeof(DDefault))]
[JsonSerializable(typeof(NavigationBarLanguage))]
[JsonSerializable(typeof(CdnList))]
[JsonSerializable(typeof(List<CdnList>))]
[JsonSerializable(typeof(Changelog))]
[JsonSerializable(typeof(CurrentGameInfo))]
[JsonSerializable(typeof(IndexDefault))]
[JsonSerializable(typeof(Download))]
[JsonSerializable(typeof(Experiment))]
[JsonSerializable(typeof(PreviousGameInfo))]
[JsonSerializable(typeof(ResourceChunk))]
[JsonSerializable(typeof(ResourcesDiff))]
[JsonSerializable(typeof(ResourcesGray))]
[JsonSerializable(typeof(ResourcesLogin))]
[JsonSerializable(typeof(RHIOptionList))]
[JsonSerializable(typeof(WavesIndex))]
[JsonSerializable(typeof(Text))]
[JsonSerializable(typeof(ZhHans))]
public partial class GameLauncherSourceContext : JsonSerializerContext { }

public class GameLauncherSource
{
    [JsonPropertyName("default")]
    public DDefault Default { get; set; }

    [JsonPropertyName("crashInitSwitch")]
    public int CrashInitSwitch { get; set; }

    [JsonPropertyName("animateBgSwitch")]
    public int AnimateBgSwitch { get; set; }

    [JsonPropertyName("animateBackground")]
    public AnimateBackground AnimateBackground { get; set; }

    [JsonPropertyName("animateBackgroundLanguage")]
    public AnimateBackgroundLanguage AnimateBackgroundLanguage { get; set; }

    [JsonPropertyName("navigationBarSwitch")]
    public int NavigationBarSwitch { get; set; }

    [JsonPropertyName("navigationBarLanguage")]
    public NavigationBarLanguage NavigationBarLanguage { get; set; }

    [JsonPropertyName("resourcesGray")]
    public ResourcesGray ResourcesGray { get; set; }
}

public class AnimateBackground
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("md5")]
    public string Md5 { get; set; }

    [JsonPropertyName("frameRate")]
    public int FrameRate { get; set; }

    [JsonPropertyName("durationInSecond")]
    public int DurationInSecond { get; set; }
}

public class AnimateBackgroundLanguage
{
    [JsonPropertyName("zh-Hans")]
    public ZhHans ZhHans { get; set; }
}

public class DDefault
{
    [JsonPropertyName("cdnList")]
    public List<CdnList> CdnList { get; set; }

    [JsonPropertyName("changelog")]
    public Changelog Changelog { get; set; }

    [JsonPropertyName("installer")]
    public string Installer { get; set; }

    [JsonPropertyName("installerMD5")]
    public string InstallerMD5 { get; set; }

    [JsonPropertyName("installerSize")]
    public int InstallerSize { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }
}

public class NavigationBarLanguage
{
    [JsonPropertyName("zh-Hans")]
    public string ZhHans { get; set; }

    [JsonPropertyName("en")]
    public string En { get; set; }

    [JsonPropertyName("ja")]
    public string Ja { get; set; }

    [JsonPropertyName("ko")]
    public string Ko { get; set; }

    [JsonPropertyName("zh-Hant")]
    public string ZhHant { get; set; }

    [JsonPropertyName("fr")]
    public string Fr { get; set; }

    [JsonPropertyName("de")]
    public string De { get; set; }

    [JsonPropertyName("es")]
    public string Es { get; set; }

    [JsonPropertyName("ru")]
    public string Ru { get; set; }

    [JsonPropertyName("pt")]
    public string Pt { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("vi")]
    public string Vi { get; set; }

    [JsonPropertyName("th")]
    public string Th { get; set; }
}

public class ZhHans
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("md5")]
    public string Md5 { get; set; }

    [JsonPropertyName("frameRate")]
    public int FrameRate { get; set; }

    [JsonPropertyName("durationInSecond")]
    public int DurationInSecond { get; set; }
}
