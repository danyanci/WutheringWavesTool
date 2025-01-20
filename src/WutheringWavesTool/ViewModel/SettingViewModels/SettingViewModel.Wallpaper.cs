namespace WutheringWavesTool.ViewModel;

partial class SettingViewModel
{
    public IWallpaperService WallpaperService { get; }

    [ObservableProperty]
    public partial ObservableCollection<WallpaperModel> Images { get; set; } = new();

    private async Task InitWallpaperAsync()
    {
        try
        {
            await foreach (var a in WallpaperService.GetFilesAsync(this.CTS.Token))
            {
                this.Images.Add(a);
            }
        }
        catch (TaskCanceledException) { }
    }
}
