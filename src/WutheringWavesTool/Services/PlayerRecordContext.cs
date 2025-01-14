using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using WutheringWavesTool.Common;
using WutheringWavesTool.Pages.Dialogs;
using WutheringWavesTool.Services.Contracts;
using WutheringWavesTool.ViewModel.DialogViewModels;

namespace WutheringWavesTool.Services;

public class PlayerRecordContext : Contracts.IPlayerRecordContext
{
    public IDialogManager DialogManager { get; }

    public IServiceScope Scope { get; private set; }

    public ITipShow TipShow { get; private set; }

    public PlayerRecordContext(IDialogManager dialogManager, ITipShow tipShow)
    {
        DialogManager = dialogManager;
        TipShow = tipShow;
    }

    public void SetScope(IServiceScope scope)
    {
        this.Scope = scope;
    }

    public async Task<string> ShowInputRecordAsync(object data)
    {
        var dialog = new InputRecordCardDialog();
        dialog.ViewModel = new InputRecordCardViewModel(Scope);
        dialog.SetData(data);
        dialog.XamlRoot = DialogManager.Root;
        DialogManager.SetDialog(dialog);
        await dialog.ShowAsync();
        return dialog.ViewModel.Link;
    }
}
