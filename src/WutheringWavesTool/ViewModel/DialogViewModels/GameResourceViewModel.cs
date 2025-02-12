namespace WutheringWavesTool.ViewModel.DialogViewModels;

public sealed partial class GameResourceViewModel : ViewModelBase
{
    public GameResourceViewModel(IViewFactorys viewFactorys)
    {
        ViewFactorys = viewFactorys;
    }

    public IGameContext GameContext { get; private set; }
    public IViewFactorys ViewFactorys { get; }

    [ObservableProperty]
    public partial string GameFilesSize { get; set; }

    [ObservableProperty]
    public partial string GameProdSize { get; set; }

    internal void SetData(string contextName)
    {
        this.GameContext = Instance.Service.GetRequiredKeyedService<IGameContext>(contextName);
    }

    [RelayCommand]
    void Close()
    {
        ViewFactorys.AppContext.CloseDialog();
    }

    [RelayCommand]
    async Task Loaded()
    {
        var result = await GameContext.GameLocalConfig.GetConfigAsync(
            GameLocalSettingName.GameLauncherBassFolder
        );
        var prodFolder = await GameContext.GameLocalConfig.GetConfigAsync(
            GameLocalSettingName.ProdDownloadFolderPath
        );
        long gameSize = 0L;
        long prodSize = 0L;
        var files = Directory.GetFiles(result, "*", SearchOption.TopDirectoryOnly).ToList();
        await Task.Run(() =>
        {
            foreach (
                var item in Directory.GetDirectories(result, "*", SearchOption.TopDirectoryOnly)
            )
            {
                if (item == prodFolder)
                {
                    continue;
                }
                else
                {
                    var file = Directory.GetFiles(item, "*", SearchOption.AllDirectories);
                    files.AddRange(file);
                }
            }
            foreach (var item in files)
            {
                gameSize += new FileInfo(item).Length;
            }

            if (Directory.Exists(prodFolder))
            {
                var files = Directory.GetFiles(prodFolder, "*.*", SearchOption.AllDirectories);
                foreach (var item in files)
                {
                    prodSize += new FileInfo(item).Length;
                }
            }
        });
        GameFilesSize = $"{(gameSize / (1024.0 * 1024.0 * 1024.0)):F2} GB";
        GameProdSize = $"{(prodSize / (1024.0 * 1024.0 * 1024.0)):F2} GB";
    }
}
