using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Waves.Api.Models.Record;
using WutheringWavesTool.Common;
using WutheringWavesTool.Models.Args;
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

    private void ListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        this.ViewModel.Args = new(CreateRecordType.SelectItemOpen)
        {
            Link = null,
            Cache = e.ClickedItem as RecordCacheDetily,
        };
        this.ViewModel.DialogManager.Close();
    }
}
