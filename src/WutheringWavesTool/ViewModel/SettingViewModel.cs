using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using WutheringWavesTool.Common;
using WutheringWavesTool.Services.Contracts;

namespace WutheringWavesTool.ViewModel;

public sealed partial class SettingViewModel : ViewModelBase
{
    public SettingViewModel(IWallpaperService wallpaperService)
    {
        WallpaperService = wallpaperService;
    }

    [RelayCommand]
    async Task Loaded()
    {
        await InitWallpaperAsync();
    }
}
