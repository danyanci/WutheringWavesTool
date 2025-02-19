using WutheringWavesTool.Services.DialogServices;

namespace WutheringWavesTool.ViewModel.DialogViewModels;

public sealed partial class SelectWallpaperViewModel : DialogViewModelBase
{
    public SelectWallpaperViewModel(
        [FromKeyedServices(nameof(MainDialogService))] IDialogManager dialogManager,
        IWallpaperService wallpaperService,
        IPickersService pickersService
    )
        : base(dialogManager)
    {
        WallpaperService = wallpaperService;
        PickersService = pickersService;
    }

    public IWallpaperService WallpaperService { get; }
    public IPickersService PickersService { get; }

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

    [RelayCommand]
    async Task SelectWallpaper()
    {
        var file = await PickersService.GetFileOpenPicker([".jpg", ".png"]);
        if (file == null)
            return;
        await WallpaperService.SetWrallpaper(file.Path);
    }
}
