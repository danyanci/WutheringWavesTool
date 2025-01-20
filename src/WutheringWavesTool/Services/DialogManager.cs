namespace WutheringWavesTool.Services;

public class DialogManager : IDialogManager
{
    ContentDialog _dialog = null;
    public XamlRoot Root { get; private set; }

    public void Close()
    {
        if (_dialog == null)
            return;
        _dialog.Hide();
    }

    public void SetRoot(XamlRoot root)
    {
        this.Root = root;
    }

    public void SetDialog(ContentDialog contentDialog)
    {
        this._dialog = contentDialog;
    }
}
