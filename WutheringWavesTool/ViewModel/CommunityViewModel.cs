using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Waves.Api.Models.Messanger;
using WinUIEx;
using WutheringWavesTool.Common;
using WutheringWavesTool.Services.Contracts;

namespace WutheringWavesTool.ViewModel;

public partial class CommunityViewModel : ViewModelBase
{
    public CommunityViewModel(IWindowFactorys windowFactorys)
    {
        WindowFactorys = windowFactorys;
        RegisterMessanger();
    }

    private void RegisterMessanger()
    {
        this.Messenger.Register<GeeSuccessMessanger>(this, GeeSuccessMessangerMethod);
    }

    private async void GeeSuccessMessangerMethod(object recipient, GeeSuccessMessanger message) { }

    public IWindowFactorys WindowFactorys { get; }

    [RelayCommand]
    void ShowGetGeet()
    {
        var win = WindowFactorys.CreateGeetWindow();
        win.AppWindowApp.Show();
    }
}
