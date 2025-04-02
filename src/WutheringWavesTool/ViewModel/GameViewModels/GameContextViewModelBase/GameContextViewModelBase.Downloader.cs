namespace WutheringWavesTool.ViewModel.GameViewModels;

partial class GameContextViewModelBase
{
    [ObservableProperty]
    public partial double MaxProgressValue { get; set; }

    [ObservableProperty]
    public partial double CurrentProgressValue { get; set; }

    [ObservableProperty]
    public partial string UpdateString { get; set; }

    private async Task GameContext_GameContextOutput(object sender, GameContextOutputArgs args)
    {
        await AppContext.TryInvokeAsync(() =>
        {
            this.MaxProgressValue = args.TotalSize;
            this.CurrentProgressValue = args.CurrentSize;
            this.UpdateString =
                args.Type == Waves.Core.Models.Enums.GameContextActionType.Verify
                    ? $"校验速度:{Math.Round(args.VerifySpeed / 1024 / 1024, 2)}MB"
                    : $"下载速度:{Math.Round(args.DownloadSpeed / 1024 / 1024, 2)}MB";
        });
    }

    [RelayCommand]
    async Task PauseDownloadTask()
    {
        await this.GameContext.PauseDownloadAsync();
    }

    [RelayCommand]
    async Task ResumeDownloadTask()
    {
        await this.GameContext.ResumeDownloadAsync();
    }
}
