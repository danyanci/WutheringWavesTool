using System.Text.Json.Serialization;

namespace Waves.Api.Models.Communitys.DataCenter
{
    public class BoxList
    {
        [JsonPropertyName("boxName")]
        public string BoxName { get; set; }

        [JsonPropertyName("num")]
        public int Num { get; set; }
    }

    public class PhantomBoxList
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("num")]
        public int Num { get; set; }
    }

    public class GamerBassString
    {
        /// <summary>
        ///
        /// </summary>
        [JsonPropertyName("code")]
        public int Code { get; set; }

        /// <summary>
        ///
        /// </summary>
        [JsonPropertyName("data")]
        public string Data { get; set; }

        /// <summary>
        /// 请求成功
        /// </summary>
        [JsonPropertyName("msg")]
        public string Msg { get; set; }

        /// <summary>
        ///
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }

    public class GamerDataBool
    {
        /// <summary>
        ///
        /// </summary>
        [JsonPropertyName("code")]
        public int Code { get; set; }

        /// <summary>
        ///
        /// </summary>
        [JsonPropertyName("data")]
        public bool Data { get; set; }

        /// <summary>
        /// 请求成功
        /// </summary>
        [JsonPropertyName("msg")]
        public string Msg { get; set; }

        /// <summary>
        ///
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }

    public class GamerBassData
    {
        [JsonPropertyName("achievementCount")]
        public int AchievementCount { get; set; }

        [JsonPropertyName("achievementStar")]
        public int AchievementStar { get; set; }

        [JsonPropertyName("activeDays")]
        public int ActiveDays { get; set; }

        [JsonPropertyName("bigCount")]
        public int BigCount { get; set; }

        [JsonPropertyName("boxList")]
        public List<BoxList> BoxList { get; set; }

        [JsonPropertyName("chapterId")]
        public int ChapterId { get; set; }

        [JsonPropertyName("creatTime")]
        public long CreatTime { get; set; }

        [JsonPropertyName("energy")]
        public int Energy { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("level")]
        public int Level { get; set; }

        [JsonPropertyName("liveness")]
        public int Liveness { get; set; }

        [JsonPropertyName("livenessMaxCount")]
        public int LivenessMaxCount { get; set; }

        [JsonPropertyName("livenessUnlock")]
        public bool LivenessUnlock { get; set; }

        [JsonPropertyName("maxEnergy")]
        public int MaxEnergy { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("phantomBoxList")]
        public List<PhantomBoxList> PhantomBoxList { get; set; }

        [JsonPropertyName("roleNum")]
        public int RoleNum { get; set; }

        [JsonPropertyName("showToGuest")]
        public bool ShowToGuest { get; set; }

        [JsonPropertyName("smallCount")]
        public int SmallCount { get; set; }

        [JsonPropertyName("treasureBoxList")]
        public List<TreasureBoxList> TreasureBoxList { get; set; }

        [JsonPropertyName("worldLevel")]
        public int WorldLevel { get; set; }

        [JsonPropertyName("weeklyInstCount")]
        public int WeeklyInstCount { get; set; }

        [JsonPropertyName("weeklyInstCountLimit")]
        public int WeeklyInstCountLimit { get; set; }

        [JsonPropertyName("weeklyInstIconUrl")]
        public string WeeklyInstIconUrl { get; set; }

        [JsonPropertyName("weeklyInstTitle")]
        public string WeeklyInstTitle { get; set; }
    }

    public class TreasureBoxList
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("num")]
        public int Num { get; set; }
    }
}
