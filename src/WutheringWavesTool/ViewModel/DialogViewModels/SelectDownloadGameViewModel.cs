using WutheringWavesTool.Services.DialogServices;
using static WutheringWavesTool.Controls.LayeredProgressBar;

namespace WutheringWavesTool.ViewModel.DialogViewModels;

public partial class SelectDownloadGameViewModel : DialogViewModelBase
{
    public SelectDownloadGameViewModel(
        [FromKeyedServices(nameof(MainDialogService))] IDialogManager dialogManager,
        IPickersService pickersService
    )
        : base(dialogManager)
    {
        MaxValue = 100;
        PickersService = pickersService;
    }

    [ObservableProperty]
    public partial double MaxValue { get; set; }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial bool ShowBar { get; set; } = false;

    [ObservableProperty]
    public partial ObservableCollection<LayerData> BarValues { get; set; }

    [ObservableProperty]
    public partial string FolderPath { get; set; }
    public IPickersService PickersService { get; }
    public IGameContext GameContext { get; private set; }

    [RelayCommand]
    async Task SelectFolder()
    {
        this.IsLoading = true;
        var result = await this.PickersService.GetFolderPicker();
        if (result == null)
        {
            return;
        }
        this.FolderPath = result.Path;
        string? rootPath = Path.GetPathRoot(this.FolderPath);
        DriveInfo? selectedDrive = DriveInfo
            .GetDrives()
            .FirstOrDefault(drive =>
                drive.Name.Equals(rootPath, StringComparison.OrdinalIgnoreCase)
            );
        if (selectedDrive == null)
            return;
        double totalSizeMB = (double)selectedDrive.TotalSize / (1024 * 1024 * 1024);
        double freeSpaceMB = (double)selectedDrive.TotalFreeSpace / (1024 * 1024 * 1024);
        double usedSpaceMB = totalSizeMB - freeSpaceMB;
        MaxValue = totalSizeMB;
        ShowBar = true;
        this.IsLoading = false;
        var launcher = await this.GameContext.GetGameLauncherSourceAsync(this.CTS.Token);
        var updateSize = usedSpaceMB + launcher.ResourceDefault.Config.Size / 1024 / 1024 / 1024;
        this.BarValues = new ObservableCollection<LayerData>()
        {
            new LayerData()
            {
                Label = "总容量",
                Color = new SolidColorBrush(Colors.LightGreen),
                Value = totalSizeMB,
            },
            new LayerData()
            {
                Label = "已用容量",
                Color = new SolidColorBrush(Colors.Purple),
                Value = usedSpaceMB,
            },
            new LayerData()
            {
                Label = "下载后增量",
                Color = new SolidColorBrush(Colors.Red),
                Value = usedSpaceMB + launcher.ResourceDefault.Config.Size / 1024 / 1024 / 1024,
            },
        };
    }

    internal void SetData(Type type)
    {
        var name = type.Name;
        this.GameContext = Instance.Service.GetRequiredKeyedService<IGameContext>(name);
    }
}
