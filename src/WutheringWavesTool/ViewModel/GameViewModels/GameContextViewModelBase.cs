using SqlSugar;

namespace WutheringWavesTool.ViewModel.GameViewModels
{
    public abstract partial class GameContextViewModelBase : ViewModelBase
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

        [ObservableProperty]
        public partial double MaxProgress { get; set; }

        [ObservableProperty]
        public partial double Progress { get; set; }

        private async Task GameContext_GameContextOutput(object sender, GameContextOutputArgs args)
        {
            await AppContext.TryInvokeAsync(() =>
            {
                if (args.Type == Waves.Core.Models.Enums.GameContextActionType.Work)
                {
                    this.MaxProgress = args.MaxSize;
                    this.Progress = args.CurrentSize;
                }
                Debug.WriteLine(
                    $"增量检查进度:{args.Progress},{args.CurrentSize}/{args.MaxSize},下载速度:{args.Speed}"
                );
            });
        }

        [ObservableProperty]
        public partial Visibility GameInstallBthVisibility { get; set; }

        [ObservableProperty]
        public partial string BthContent { get; set; }

        [ObservableProperty]
        public partial string BthIcon { get; set; }

        /// <summary>
        /// 按钮类型,1为安装游戏,2为继续下载游戏,3为开始游戏
        /// </summary>
        private int _bthType = 0;

        [RelayCommand]
        async Task Loaded()
        {
            var status = await this.GameContext.GetGameContextStatusAsync(this.CTS.Token);
            if (!status.IsGameExists && !status.IsGameInstalled)
            {
                ShowSelectInstallBth();
            }
            else if (status.IsGameExists && !status.IsGameInstalled)
            {
                ShowGameDownloadBth();
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
            BthContent = "安装游戏";
            BthIcon = "\uE896";
        }

        /// <summary>
        /// 显示继续下载
        /// </summary>
        private void ShowGameDownloadBth()
        {
            _bthType = 2;
            BthContent = "继续安装";
            BthIcon = "\uE896";
        }

        public abstract Task LoadAfter();
    }
}
