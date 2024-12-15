using System.Text.Json.Serialization;

namespace Waves.Api.Models.Communitys.DataCenter;

public class DifficultyList
{
    [JsonPropertyName("difficulty")]
    public int Difficulty { get; set; }

    [JsonPropertyName("difficultyName")]
    public string DifficultyName { get; set; }

    [JsonPropertyName("towerAreaList")]
    public List<TowerAreaList> TowerAreaList { get; set; }
}

public class FloorList
{
    [JsonPropertyName("floor")]
    public int Floor { get; set; }

    [JsonPropertyName("picUrl")]
    public string PicUrl { get; set; }

    [JsonPropertyName("roleList")]
    public List<TowerIndexRoleList> RoleList { get; set; }

    [JsonPropertyName("star")]
    public int Star { get; set; }
}

public class TowerIndexRoleList
{
    [JsonPropertyName("iconUrl")]
    public string IconUrl { get; set; }

    [JsonPropertyName("roleId")]
    public int RoleId { get; set; }
}

public class GamerTowerIndexData
{
    [JsonPropertyName("difficultyList")]
    public List<DifficultyList> DifficultyList { get; set; }

    [JsonPropertyName("isUnlock")]
    public bool IsUnlock { get; set; }

    [JsonPropertyName("seasonEndTime")]
    public int SeasonEndTime { get; set; }
}

public class TowerAreaList
{
    [JsonPropertyName("areaId")]
    public int AreaId { get; set; }

    [JsonPropertyName("areaName")]
    public string AreaName { get; set; }

    [JsonPropertyName("floorList")]
    public List<FloorList> FloorList { get; set; }

    [JsonPropertyName("maxStar")]
    public int MaxStar { get; set; }

    [JsonPropertyName("star")]
    public int Star { get; set; }
}
