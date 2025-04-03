namespace WutheringWavesTool.ViewModel.GameViewModels;

partial class GameContextViewModelBase
{
    [ObservableProperty]
    public partial double MaxProgressValue { get; set; }

    [ObservableProperty]
    public partial double CurrentProgressValue { get; set; }

    private async Task GameContext_GameContextOutput(object sender, GameContextOutputArgs args)
    {
        await AppContext.TryInvokeAsync(() =>
        {
            if (
                args.Type == Waves.Core.Models.Enums.GameContextActionType.Download
                || args.Type == Waves.Core.Models.Enums.GameContextActionType.Verify
            )
            {
                this.MaxProgressValue = args.TotalSize;
                this.CurrentProgressValue = args.CurrentSize;
                if (args.Type == Waves.Core.Models.Enums.GameContextActionType.Verify)
                {
                    this.BottomBarContent =
                        $"校验速度:{Math.Round(args.VerifySpeed / 1024 / 1024, 2)}MB";
                }
                else if (args.Type == Waves.Core.Models.Enums.GameContextActionType.Download)
                {
                    this.BottomBarContent =
                        $"下载速度:{Math.Round(args.DownloadSpeed / 1024 / 1024, 2)}MB，剩余时间"
                        + $"{(int)args.RemainingTime.TotalHours:00}:{args.RemainingTime.Minutes:00}:{args.RemainingTime.Seconds:00}";
                }
                ShowGameDownloadingBth();
            }
        });
    }

    [RelayCommand]
    async Task PauseDownloadTask()
    {
        var status = await this.GameContext.GetGameContextStatusAsync(this.CTS.Token);
        if (status.IsPause)
        {
            if (await this.GameContext.ResumeDownloadAsync())
            {
                this.PauseIcon = "\uE769";
            }
        }
        else
        {
            if (await this.GameContext.PauseDownloadAsync())
            {
                this.PauseIcon = "\uE768";
            }
        }
    }
}
