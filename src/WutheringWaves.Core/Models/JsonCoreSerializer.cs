using System.Text.Json.Serialization;

namespace WutheringWaves.Core.Models;

[JsonSerializable(typeof(GameLauncherModel))]
public partial class JsonCoreSerializer : JsonSerializerContext { }
