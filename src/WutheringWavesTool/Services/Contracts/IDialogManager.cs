using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WutheringWavesTool.Common;

namespace WutheringWavesTool.Services.Contracts;

public interface IDialogManager
{
    public XamlRoot Root { get; }

    public void SetRoot(XamlRoot root);

    public void SetDialog(ContentDialog contentDialog);
    public void Close();
}
