using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WutheringWavesTool.Common;
using WutheringWavesTool.Services.Contracts;

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
