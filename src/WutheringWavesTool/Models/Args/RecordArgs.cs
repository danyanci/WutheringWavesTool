using System.Collections;
using System.Collections.Generic;
using Waves.Api.Models.Enums;
using Waves.Api.Models.Record;
using Waves.Api.Models.Wrappers;

namespace WutheringWavesTool.Models.Args;

public class RecordArgs
{
    public RecordRequest Request { get; set; }

    public IEnumerable<int> Roles { get; set; }

    public IEnumerable<int> Weapons { get; set; }
    public CardPoolType Type { get; internal set; }
}
