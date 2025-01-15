using System.Text.Json.Serialization;
using Waves.Api.Models.Wrappers;

namespace Waves.Api.Models.Record;

public class RecordCacheDetily
{
    public RecordCacheDetily(
        Guid guid,
        string name,
        IEnumerable<RecordCardItemWrapper> roleActivityItems,
        IEnumerable<RecordCardItemWrapper> weaponsActivityItems,
        IEnumerable<RecordCardItemWrapper> roleResidentItems,
        IEnumerable<RecordCardItemWrapper> weaponsResidentItems,
        IEnumerable<RecordCardItemWrapper> beginnerItems,
        IEnumerable<RecordCardItemWrapper> beginnerChoiceItems,
        IEnumerable<RecordCardItemWrapper> gratitudeOrientationItems
    )
    {
        this.Guid = guid.ToString();
        Name = name;
        RoleActivityItems = roleActivityItems;
        WeaponsActivityItems = weaponsActivityItems;
        RoleResidentItems = roleResidentItems;
        WeaponsResidentItems = weaponsResidentItems;
        BeginnerItems = beginnerItems;
        BeginnerChoiceItems = beginnerChoiceItems;
        GratitudeOrientationItems = gratitudeOrientationItems;
    }

    [JsonPropertyName(nameof(Guid))]
    public string Guid { get; set; }

    [JsonPropertyName(nameof(Name))]
    public string Name { get; set; }

    [JsonPropertyName(nameof(RoleActivityItems))]
    public IEnumerable<RecordCardItemWrapper> RoleActivityItems { get; set; }

    [JsonPropertyName(nameof(WeaponsActivityItems))]
    public IEnumerable<RecordCardItemWrapper> WeaponsActivityItems { get; set; }

    [JsonPropertyName(nameof(RoleResidentItems))]
    public IEnumerable<RecordCardItemWrapper> RoleResidentItems { get; set; }

    [JsonPropertyName(nameof(WeaponsResidentItems))]
    public IEnumerable<RecordCardItemWrapper> WeaponsResidentItems { get; set; }

    [JsonPropertyName(nameof(BeginnerItems))]
    public IEnumerable<RecordCardItemWrapper> BeginnerItems { get; set; }

    [JsonPropertyName(nameof(BeginnerChoiceItems))]
    public IEnumerable<RecordCardItemWrapper> BeginnerChoiceItems { get; set; }

    [JsonPropertyName(nameof(GratitudeOrientationItems))]
    public IEnumerable<RecordCardItemWrapper> GratitudeOrientationItems { get; set; }
}
