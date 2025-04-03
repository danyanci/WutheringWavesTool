using SqlSugar;

namespace WutheringWavesTool.ViewModel.GameViewModels
{
    public abstract partial class GameContextViewModelBase : ViewModelBase, IDisposable
    {
        public IGameContext GameContext { get; }
        public IDialogManager DialogManager { get; }
        public IAppContext<App> AppContext { get; }

        protected GameContextViewModelBase(
            IGameContext gameContext,
            IDialogManager dialogManager,
            IAppContext<App> appContext
        )
        {
            GameContext = gameContext;
            DialogManager = dialogManager;
            AppContext = appContext;
            GameContext.GameContextOutput += GameContext_GameContextOutput;
        }

        #region MyRegion

        /// <summary>
        /// 选择下载路径显示
        /// </summary>
        [ObservableProperty]
        public partial Visibility GameInstallBthVisibility { get; set; } = Visibility.Collapsed;

        /// <summary>
        /// 定位游戏路径显示
        /// </summary>
        [ObservableProperty]
        public partial Visibility GameInputFolderBthVisibility { get; set; } = Visibility.Collapsed;

        /// <summary>
        /// 游戏下载中
        /// </summary>
        [ObservableProperty]
        public partial Visibility GameDownloadingBthVisibility { get; set; } = Visibility.Collapsed;

        #endregion

        [ObservableProperty]
        public partial string PauseIcon { get; set; }

        [ObservableProperty]
        public partial string BottomBarContent { get; set; }

        /// <summary>
        /// 按钮类型,1为安装游戏,2为下载游戏,3为开始游戏
        /// </summary>
        private int _bthType = 0;
        private bool disposedValue;

        [RelayCommand]
        async Task Loaded()
        {
            var status = await this.GameContext.GetGameContextStatusAsync(this.CTS.Token);
            if (!status.IsGameExists && !status.IsGameInstalled)
            {
                ShowSelectInstallBth();
            }
            if (status.IsGameExists && !status.IsGameInstalled)
            {
                ShowGameDownloadBth();
            }
            if (
                status.IsGameExists
                && !status.IsGameInstalled
                && (status.IsPause || status.IsAction)
            )
            {
                ShowGameDownloadingBth();
                if (status.IsPause)
                {
                    this.PauseIcon = "\uE768";
                }
                else
                {
                    this.PauseIcon = "\uE769";
                }
            }
            await LoadAfter();
        }

        [RelayCommand]
        async Task ShowSelectInstallFolder()
        {
            if (_bthType == 1)
            {
                var result = await DialogManager.ShowSelectInstallFolderAsync(
                    this.GameContext.ContextType
                );
                if (result.Result == ContentDialogResult.None)
                {
                    return;
                }
                await this.GameContext.StartDownloadTaskAsync(
                    result.InstallFolder,
                    result.Launcher
                );
            }
            else
            {
                var launcher = await GameContext.GetGameLauncherSourceAsync(this.CTS.Token);
                await this.GameContext.StartDownloadTaskAsync(
                    await GameContext.GameLocalConfig.GetConfigAsync(
                        GameLocalSettingName.GameLauncherBassFolder
                    ),
                    launcher
                );
            }
        }

        /// <summary>
        /// 显示
        /// </summary>
        private void ShowSelectInstallBth()
        {
            _bthType = 1;
            GameInputFolderBthVisibility = Visibility.Visible;
            GameInstallBthVisibility = Visibility.Visible;
            BottomBarContent = "游戏文件不存在，请找到窗口右下角选择游戏下载路径或定位游戏";
        }

        private void ShowGameDownloadingBth()
        {
            _bthType = 2;
            if (GameDownloadingBthVisibility == Visibility.Visible)
                return;
            GameInputFolderBthVisibility = Visibility.Collapsed;
            GameInstallBthVisibility = Visibility.Collapsed;
            GameDownloadingBthVisibility = Visibility.Visible;
            PauseIcon = "\uE769";
        }

        /// <summary>
        /// 显示继续下载
        /// </summary>
        private void ShowGameDownloadBth()
        {
            _bthType = 2;
            GameInputFolderBthVisibility = Visibility.Collapsed;
            GameInstallBthVisibility = Visibility.Visible;
            BottomBarContent = "请点击右下角继续更新游戏";
        }

        public abstract Task LoadAfter();

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    GameContext.GameContextOutput -= GameContext_GameContextOutput;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
