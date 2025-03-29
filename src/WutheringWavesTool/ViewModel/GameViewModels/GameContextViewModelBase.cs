namespace WutheringWavesTool.ViewModel.GameViewModels
{
    public abstract partial class GameContextViewModelBase : ViewModelBase
    {
        public IGameContext GameContext { get; }
        public IDialogManager DialogManager { get; }

        protected GameContextViewModelBase(IGameContext gameContext, IDialogManager dialogManager)
        {
            GameContext = gameContext;
            DialogManager = dialogManager;
        }

        [ObservableProperty]
        public partial Visibility GameInstallBthVisibility { get; set; }

        [RelayCommand]
        async Task Loaded()
        {
            var status = await this.GameContext.GetGameContextStatusAsync(this.CTS.Token);
            if (status.IsGameExists)
            {
                ShowSelectInstallBth();
            }
            await LoadAfter();
        }

        [RelayCommand]
        async Task ShowSelectInstallFolder()
        {
            var result = await DialogManager.ShowSelectInstallFolderAsync(
                this.GameContext.ContextType
            );
        }

        private void ShowSelectInstallBth()
        {
            GameInstallBthVisibility = Visibility.Visible;
        }

        public abstract Task LoadAfter();
    }
}
