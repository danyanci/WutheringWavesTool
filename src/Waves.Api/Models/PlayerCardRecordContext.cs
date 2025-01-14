using System.Text.Json.Serialization;
using Waves.Api.Models.Communitys;
using Waves.Api.Models.Record;

namespace Waves.Api.Models;

[JsonSerializable(typeof(Datum))]
[JsonSerializable(typeof(PlayerCard))]
[JsonSerializable(typeof(PlayerReponse))]
[JsonSerializable(typeof(List<Datum>))]
[JsonSerializable(typeof(RecordRequest))]
[JsonSerializable(typeof(FiveGroupData))]
[JsonSerializable(typeof(FiveGroupConfig))]
[JsonSerializable(typeof(FiveMap))]
[JsonSerializable(typeof(PoolList))]
[JsonSerializable(typeof(FiveGroupModel))]
[JsonSerializable(typeof(VersionPool))]
[JsonSerializable(typeof(CommunityRoleData))]
[JsonSerializable(typeof(List<CommunityRoleData>))]
[JsonSerializable(typeof(Prop))]
[JsonSerializable(typeof(Tag))]
public partial class PlayerCardRecordContext : JsonSerializerContext { }
