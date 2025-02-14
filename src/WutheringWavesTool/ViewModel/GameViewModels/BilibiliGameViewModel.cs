using WutheringWavesTool.Services.DialogServices;

namespace WutheringWavesTool.ViewModel.GameViewModels;

public sealed partial class BilibiliGameViewModel : GameViewModelBase
{
    public BilibiliGameViewModel(
        [FromKeyedServices(nameof(BilibiliGameContext))] IGameContext gameContext,
        IPickersService pickersService,
        IAppContext<App> appContext,
        ITipShow tipShow,
        [FromKeyedServices(nameof(MainDialogService))] IDialogManager dialogManager
    )
        : base(gameContext, pickersService, appContext, tipShow, dialogManager) { }

    public override async Task RemoveGameResource(DeleteGameResource resourceMessage) { }

    public override async Task ShowGameResourceMethod()
    {
        await DialogManager.ShowGameResourceDialogAsync(nameof(BilibiliGameContext));
    }
}
