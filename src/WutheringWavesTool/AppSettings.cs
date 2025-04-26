using SqlSugar;

namespace WutheringWavesTool;

public class AppSettings
{
    public static ApplicationDataContainer LocalSetting =>
        Windows.Storage.ApplicationData.Current.LocalSettings;

    public static ISqlSugarClient SettingDB =>
        new SqlSugarScope(
            new ConnectionConfig()
            {
                ConnectionString = $"DataSource={App.BassFolder}\\System.db",
                DbType = DbType.Sqlite,
            }
        );

    public static string? Token
    {
        get => ReadDb();
        set => WriteDb(value);
    }

    public static string? TokenId
    {
        get => ReadDb();
        set => WriteDb(value);
    }

    public static string? WallpaperPath
    {
        get => ReadDb();
        set => WriteDb(value);
    }

    public static string? AppTheme
    {
        get => ReadDb();
        set => WriteDb(value);
    }

    internal static string? Read([CallerMemberName] string Path = null)
    {
        if (LocalSetting.Values.TryGetValue(Path, out object value))
        {
            return value.ToString();
        }
        return null;
    }

    internal static string? ReadDb([CallerMemberName] string key = null)
    {
        SettingDB.CodeFirst.InitTables<LocalSettings>();
        var result = SettingDB.Queryable<LocalSettings>().Where(x => x.Key == key);
        if (result.Count() == 1)
        {
            return result.First().Value;
        }
        return null;
    }

    internal static void WriteDb(string? value, [CallerMemberName] string key = null)
    {
        SettingDB.CodeFirst.InitTables<LocalSettings>();
        var result = SettingDB.Queryable<LocalSettings>().Where(x => x.Key == key).First();

        if (result != null)
        {
            result.Value = value;
            SettingDB.Updateable(result).ExecuteCommand();
        }
        else
        {
            var newSetting = new LocalSettings { Key = key, Value = value };
            SettingDB.Insertable(newSetting).ExecuteCommand();
        }
    }

    internal static void Write(string? value, [CallerMemberName] string path = null)
    {
        if (value == null)
            return;
        LocalSetting.Values[path] = value;
    }
}
