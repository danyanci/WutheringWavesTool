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
    public Task<Result> GetDialogResultAsync<T, Result>(object? data)
        where T : ContentDialog, IResultDialog<Result>, new()
        where Result : new();
    public void CloseDialog();
    Task ShowWallpaperDialogAsync();
}
