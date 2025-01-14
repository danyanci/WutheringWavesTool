using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using WutheringWavesTool.Services;
using WutheringWavesTool.Services.Contracts;
using static System.Formats.Asn1.AsnWriter;

namespace WutheringWavesTool.ViewModel.DialogViewModels;

public partial class InputRecordCardViewModel : ObservableObject
{
    public InputRecordCardViewModel(IServiceScope scope)
    {
        DialogManager = scope.ServiceProvider.GetRequiredService<IDialogManager>();
        TipShow = scope.ServiceProvider.GetRequiredService<ITipShow>();
    }

    [ObservableProperty]
    public partial string Link { get; set; }

    partial void OnLinkChanged(string value)
    {
        this.InvokeCommand.NotifyCanExecuteChanged();
    }

    public bool GetIsInvoke() => !string.IsNullOrWhiteSpace(Link);

    [RelayCommand]
    public void Close()
    {
        this.Link = "";
        TipShow.ShowMessage("抽卡链接为空", Microsoft.UI.Xaml.Controls.Symbol.Clear);
        this.DialogManager.Close();
    }

    [RelayCommand(CanExecute = nameof(GetIsInvoke))]
    public void Invoke()
    {
        this.DialogManager.Close();
    }

    public IServiceScope Scope { get; }
    public IDialogManager DialogManager { get; }
    public ITipShow TipShow { get; }
}
