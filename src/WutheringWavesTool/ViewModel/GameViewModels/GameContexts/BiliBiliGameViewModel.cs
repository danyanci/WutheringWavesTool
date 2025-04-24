using WutheringWavesTool.Services.DialogServices;

namespace WutheringWavesTool.ViewModel.GameViewModels;

public class BiliBiliGameViewModel : GameContextViewModelBase
{
    public BiliBiliGameViewModel(
        [FromKeyedServices(nameof(BiliBiliGameContext))] IGameContext gameContext,
        [FromKeyedServices(nameof(MainDialogService))] IDialogManager dialogManager,
        IAppContext<App> appContext
    )
        : base(gameContext, dialogManager, appContext) { }

    public override void DisposeAfter() { }

    public override Task LoadAfter()
    {
        return Task.CompletedTask;
    }
}
