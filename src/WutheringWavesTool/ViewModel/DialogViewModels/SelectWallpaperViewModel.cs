using WutheringWavesTool.Services.DialogServices;

namespace WutheringWavesTool.ViewModel.DialogViewModels;

public sealed partial class SelectWallpaperViewModel : DialogViewModelBase
{
    public SelectWallpaperViewModel(
        [FromKeyedServices(nameof(MainDialogService))] IDialogManager dialogManager,
        IWallpaperService wallpaperService
    )
        : base(dialogManager)
    {
        WallpaperService = wallpaperService;
    }

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

    [RelayCommand]
    async Task Loaded()
    {
        await InitWallpaperAsync();
    }
}
