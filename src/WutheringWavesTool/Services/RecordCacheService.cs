using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Waves.Api.Models;
using Waves.Api.Models.Record;
using WutheringWavesTool.Services.Contracts;

namespace WutheringWavesTool.Services;

public sealed partial class RecordCacheService : IRecordCacheService
{
    public async Task<bool> CreateRecordAsync(RecordCacheDetily recordCacheDetily)
    {
        var filePath = $"{App.RecordFolder}\\{recordCacheDetily.Guid}.json";
        if (File.Exists($"{App.RecordFolder}"))
        {
            File.Delete(filePath);
        }
        using (var fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.ReadWrite))
        {
            using (var writer = new StreamWriter(fs))
            {
                try
                {
                    var json = JsonSerializer.Serialize(
                        recordCacheDetily,
                        PlayerCardRecordContext.Default.RecordCacheDetily
                    );
                    await writer.WriteAsync(json);
                    return true;
                }
                catch (System.Exception)
                {
                    await writer.FlushAsync();
                    writer.Close();
                    File.Delete(filePath);
                    return false;
                }
            }
        }
    }

    public async Task<IEnumerable<RecordCacheDetily?>> GetRecordCacheDetilyAsync()
    {
        List<RecordCacheDetily?> list = new List<RecordCacheDetily?>();
        var dirinfo = new DirectoryInfo(App.RecordFolder);
        foreach (var filePath in dirinfo.EnumerateFiles("*.json"))
        {
            using (var fs = new FileStream(filePath.FullName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(fs))
                {
                    var str = await reader.ReadToEndAsync();
                    try
                    {
                        var json = JsonSerializer.Deserialize<RecordCacheDetily>(
                            str,
                            PlayerCardRecordContext.Default.RecordCacheDetily
                        );
                        list.Add(json);
                    }
                    catch (System.Exception)
                    {
                        reader.Close();
                        continue;
                    }
                }
            }
        }
        return list;
    }
}
