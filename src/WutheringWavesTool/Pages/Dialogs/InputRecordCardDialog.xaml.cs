using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using WutheringWavesTool.Common;
using WutheringWavesTool.ViewModel.DialogViewModels;

namespace WutheringWavesTool.Pages.Dialogs;

public sealed partial class InputRecordCardDialog : ContentDialog, IDialog
{
    public InputRecordCardDialog()
    {
        this.InitializeComponent();
    }

    public InputRecordCardViewModel ViewModel { get; internal set; }

    public void SetData(object data) { }
}
