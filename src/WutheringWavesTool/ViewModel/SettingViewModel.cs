using WutheringWavesTool.Services.DialogServices;

namespace WutheringWavesTool.ViewModel;

public sealed partial class SettingViewModel : ViewModelBase
{
    public SettingViewModel(
        [FromKeyedServices(nameof(MainDialogService))] IDialogManager dialogManager
    )
    {
        DialogManager = dialogManager;
    }

    public IDialogManager DialogManager { get; }

    [RelayCommand]
    async Task Loaded() { }
}
