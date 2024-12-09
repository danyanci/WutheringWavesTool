using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using SqlSugar;

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

    /// <summary>
    /// 本地游戏版本
    /// </summary>
    public const string LocalGameResourceVersion = nameof(LocalGameResourceVersion);

    public const string LocalGameVersion = nameof(LocalGameVersion);
}

public class GameLocalConfig
{
    public string SettingPath { get; set; }

    public async Task SaveConfigAsync(string key, string value)
    {
        string connectionString = $"Data Source={SettingPath};";
        using (
            ISqlSugarClient context = new SqlSugarClient(
                new ConnectionConfig()
                {
                    ConnectionString = connectionString,
                    DbType = DbType.Sqlite,
                    IsAutoCloseConnection = true,
                }
            )
        )
        {
            context.CodeFirst.InitTables<LocalSettings>();
            var settings = new LocalSettings() { Key = key, Value = value };
            var existingSetting = (
                await context.Queryable<LocalSettings>().Where(x => x.Key == key).AnyAsync()
            );
            if (existingSetting)
            {
                await context.Updateable(settings).ExecuteCommandAsync();
            }
            else
            {
                await context.Insertable(settings).ExecuteCommandAsync();
            }
            context.Ado.CommitTran();
        }
    }

    public async Task<string> GetConfigAsync(string key)
    {
        string connectionString = $"Data Source={SettingPath};";
        using (
            SqlSugarClient context = new SqlSugarClient(
                new ConnectionConfig()
                {
                    ConnectionString = connectionString,
                    DbType = DbType.Sqlite,
                    IsAutoCloseConnection = true,
                }
            )
        )
        {
            context.CodeFirst.InitTables<LocalSettings>();
            var existingSetting = (
                await context.Queryable<LocalSettings>().Where(x => x.Key == key).FirstAsync()
            );
            if (existingSetting == null)
            {
                return null;
            }
            else
            {
                return existingSetting.Value;
            }
        }
    }
}

[SugarTable("settings")]
public class LocalSettings
{
    [SugarColumn(ColumnName = "Key", IsPrimaryKey = true)]
    public string Key { get; set; }

    [SugarColumn(ColumnName = "Value")]
    public string Value { get; set; }
}
