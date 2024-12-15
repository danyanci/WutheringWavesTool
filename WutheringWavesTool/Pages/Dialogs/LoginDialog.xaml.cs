using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using WutheringWavesTool.Common;
using WutheringWavesTool.ViewModel.DialogViewModels;

namespace WutheringWavesTool.Pages.Dialogs;

public sealed partial class LoginDialog : ContentDialog, IDialog
{
    public LoginDialog()
    {
        this.InitializeComponent();
        this.ViewModel = Instance.Service.GetRequiredService<LoginViewModel>();
    }

    public LoginViewModel ViewModel { get; }

    public void SetData(object data) { }

    private void SelectorBar_SelectionChanged(
        SelectorBar sender,
        SelectorBarSelectionChangedEventArgs args
    )
    {
        this.ViewModel.SwitchView(sender.SelectedItem.Tag.ToString());
    }
}
