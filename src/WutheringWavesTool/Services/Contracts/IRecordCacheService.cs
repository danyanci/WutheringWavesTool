using System.Collections.Generic;
using System.Threading.Tasks;
using Waves.Api.Models.Record;

namespace WutheringWavesTool.Services.Contracts;

public interface IRecordCacheService
{
    Task<bool> CreateRecordAsync(RecordCacheDetily recordCacheDetily);
    Task<IEnumerable<RecordCacheDetily?>> GetRecordCacheDetilyAsync();
}
