using System.Text.Json;
using System.Text.Json.Serialization;

namespace Waves.Core.Models;

public class GameLocalSettingName
{
    /// <summary>
    /// 游戏启动文件夹
    /// </summary>
    public const string GameLauncherBassFolder = nameof(GameLauncherBassFolder);

    /// <summary>
    /// 游戏启动可执行文件
    /// </summary>
    public const string GameLauncherBassProgram = nameof(GameLauncherBassProgram);
}

public class GameLocalConfig
{
    public string SettingPath { get; set; }

    public Dictionary<string, object> Properties { get; set; }

    public async Task<bool> RefreshAsync(CancellationToken token = default)
    {
        try
        {
            using (var fs = new StreamReader(SettingPath))
            {
                var jsonStr = await fs.ReadToEndAsync();
                var propertys = JsonSerializer.Deserialize(
                    jsonStr,
                    JsonContext.Default.DictionaryStringObject
                );
                if (propertys == null)
                    return false;
                this.Properties = propertys;
                return true;
            }
        }
        catch (Exception)
        {
            this.Properties = new Dictionary<string, object>();
            return false;
        }
    }

    public async Task<bool> SaveAsync(CancellationToken token = default)
    {
        try
        {
            using (var fs = new StreamWriter(SettingPath))
            {
                var jsonStr = JsonSerializer.Serialize(
                    this.Properties,
                    JsonContext.Default.DictionaryStringObject
                );
                await fs.WriteLineAsync(jsonStr);
                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }
}
