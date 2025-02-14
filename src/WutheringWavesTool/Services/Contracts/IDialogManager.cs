namespace WutheringWavesTool.Services.Contracts;

public interface IDialogManager
{
    public XamlRoot Root { get; }
    public void SetDialog(ContentDialog contentDialog);
    public void Close();
    public void RegisterRoot(XamlRoot root);
    public Task ShowLoginDialogAsync();
    public Task<ContentDialogResult> ShowBindGameDataAsync(string name);
    public Task ShowGameResourceDialogAsync(string contextName);

    public void CloseDialog();
    Task ShowWallpaperDialogAsync();
}
