using System.Text.Json.Serialization;
using Waves.Api.Models.Wrappers;

namespace Waves.Api.Models.Record;

public class RecordCacheDetily
{
    [JsonPropertyName(nameof(Guid))]
    public string Guid { get; set; }

    [JsonPropertyName(nameof(Name))]
    public string Name { get; set; }

    [JsonPropertyName(nameof(Time))]
    public DateTime Time { get; set; }

    [JsonPropertyName(nameof(Id))]
    public string Id { get; set; }

    [JsonPropertyName(nameof(RoleActivityItems))]
    public List<RecordCardItemWrapper> RoleActivityItems { get; set; }

    [JsonPropertyName(nameof(WeaponsActivityItems))]
    public List<RecordCardItemWrapper> WeaponsActivityItems { get; set; }

    [JsonPropertyName(nameof(RoleResidentItems))]
    public List<RecordCardItemWrapper> RoleResidentItems { get; set; }

    [JsonPropertyName(nameof(WeaponsResidentItems))]
    public List<RecordCardItemWrapper> WeaponsResidentItems { get; set; }

    [JsonPropertyName(nameof(BeginnerItems))]
    public List<RecordCardItemWrapper> BeginnerItems { get; set; }

    [JsonPropertyName(nameof(BeginnerChoiceItems))]
    public List<RecordCardItemWrapper> BeginnerChoiceItems { get; set; }

    [JsonPropertyName(nameof(GratitudeOrientationItems))]
    public List<RecordCardItemWrapper> GratitudeOrientationItems { get; set; }
}
