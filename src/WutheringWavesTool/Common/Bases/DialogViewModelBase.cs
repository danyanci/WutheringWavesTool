using WutheringWavesTool.Services.DialogServices;

namespace WutheringWavesTool.Common.Bases;

public partial class DialogViewModelBase : ViewModelBase
{
    public DialogViewModelBase(
        [FromKeyedServices(nameof(MainDialogService))] IDialogManager dialogManager
    )
    {
        DialogManager = dialogManager;
    }

    public IDialogManager DialogManager { get; }

    [RelayCommand]
    protected void Close()
    {
        DialogManager.CloseDialog();
    }
}
