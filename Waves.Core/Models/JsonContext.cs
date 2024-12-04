using System.Text.Json;
using System.Text.Json.Serialization;

namespace Waves.Core.Models
{
    [JsonSerializable(typeof(Dictionary<string, object>))]
    public partial class JsonContext : JsonSerializerContext { }
}
