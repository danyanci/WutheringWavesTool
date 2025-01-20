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
