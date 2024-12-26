using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WutheringWavesTool.ViewModel.GameViewModels;

partial class GameViewModelBase
{
    #region GameContextConfig
    [ObservableProperty]
    public partial double LimitDownloadSpeed { get; set; }

    [ObservableProperty]
    public partial bool IsDx11 { get; set; }
    #endregion

    private async Task ReadContextConfig()
    {
        var result = await GameContext.ReadContextConfigAsync(this.CTS.Token);
        this.LimitDownloadSpeed = result.LimitSpeed;
        this.IsDx11 = result.IsDx11;
    }

    async partial void OnIsDx11Changed(bool oldValue, bool newValue)
    {
        if (oldValue == newValue)
            return;
        await GameContext.SetDx11LauncheAsync(newValue);
    }

    [RelayCommand]
    async Task SetLimitSpeed()
    {
        await GameContext.SetLimitSpeedAsync(
            System.Convert.ToInt32(this.LimitDownloadSpeed),
            this.CTS.Token
        );
    }
}
