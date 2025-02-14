namespace WutheringWavesTool.Services;

public abstract class DialogManager : IDialogManager
{
    ContentDialog _dialog = null;
    public XamlRoot Root { get; private set; }

    public void Close()
    {
        if (_dialog == null)
            return;
        _dialog.Hide();
    }

    public void RegisterRoot(XamlRoot root)
    {
        this.Root = root;
    }

    public void SetDialog(ContentDialog contentDialog)
    {
        this._dialog = contentDialog;
    }

    public async Task ShowLoginDialogAsync() => await ShowDialogAsync<LoginDialog>();

    public async Task ShowGameResourceDialogAsync(string contextName)
    {
        var dialog = Instance.Service.GetRequiredService<GameResourceDialog>();
        dialog.SetData(contextName);
        dialog.XamlRoot = this.Root;
        this._dialog = dialog;
        await _dialog.ShowAsync();
    }

    public async Task<ContentDialogResult> ShowBindGameDataAsync(string name) =>
        await ShowDialogAsync<BindGameDataDialog>(name);

    public async Task ShowDialogAsync<T>()
        where T : ContentDialog, IDialog
    {
        if (_dialog != null)
            return;
        var dialog = Instance.Service.GetRequiredService<T>();
        dialog.XamlRoot = this.Root;
        this._dialog = dialog;
        await _dialog.ShowAsync();
    }

    public async Task<ContentDialogResult> ShowDialogAsync<T>(object data)
        where T : ContentDialog, IDialog
    {
        if (_dialog != null)
            return ContentDialogResult.None;
        var dialog = Instance.Service.GetRequiredService<T>();
        dialog.XamlRoot = this.Root;
        dialog.SetData(data);
        this._dialog = dialog;
        return await _dialog.ShowAsync();
    }

    public void CloseDialog()
    {
        if (_dialog == null)
            return;
        _dialog.Hide();
        _dialog = null;
        GC.Collect();
    }

    public async Task ShowWallpaperDialogAsync()
    {
        await ShowDialogAsync<SelectWallpaperDialog>(null);
    }
}
