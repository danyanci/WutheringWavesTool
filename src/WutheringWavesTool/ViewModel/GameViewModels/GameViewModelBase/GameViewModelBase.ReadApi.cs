using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Waves.Api.Models;
using WutheringWavesTool.Common;

namespace WutheringWavesTool.ViewModel.GameViewModels;

partial class GameViewModelBase
{
    [ObservableProperty]
    public partial ObservableCollection<Social> Socials { get; set; }

    public async Task GetApiDataAsync()
    {
        var launcherHeader = await this.GameContext.GetLauncherHeaderAsync(this.CTS.Token);
        if (launcherHeader != null && launcherHeader.Social != null)
        {
            this.Socials = launcherHeader.Social.ToObservableCollection();
        }
    }
}
