namespace WutheringWavesTool.Models.Dialogs;

public class SelectDownloadFolderResult
{
    public string InstallFolder { get; set; }
    public GameLauncherSource? Launcher { get; internal set; }
}
