using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Waves.Api.Models.Record;
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

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems == null)
            return;
        if (e.AddedItems[0] is RecordCacheDetily detily)
        {
            ViewModel.SelectItem = detily;
            this.ViewModel.InvokeCommand.NotifyCanExecuteChanged();
        }
    }
}
