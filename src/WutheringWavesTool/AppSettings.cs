namespace WutheringWavesTool;

public class AppSettings
{
    public static ApplicationDataContainer LocalSetting =>
        Windows.Storage.ApplicationData.Current.LocalSettings;

    public static string? Token
    {
        get => Read();
        set => Write(value);
    }

    public static string? TokenId
    {
        get => Read();
        set => Write(value);
    }

    public static string? WallpaperPath
    {
        get => Read();
        set => Write(value);
    }

    public static string? AppTheme
    {
        get => Read();
        set => Write(value);
    }

    internal static string? Read([CallerMemberName] string Path = null)
    {
        if (LocalSetting.Values.TryGetValue(Path, out object value))
        {
            return value.ToString();
        }
        return null;
    }

    internal static void Write(string? value, [CallerMemberName] string path = null)
    {
        if (value == null)
            return;
        LocalSetting.Values[path] = value;
    }
}
