using WutheringWavesTool.Services.DialogServices;

namespace WutheringWavesTool.ViewModel.GameViewModels;

public sealed partial class GlobalGameViewModel : GameContextViewModelBase
{
    public GlobalGameViewModel(
        [FromKeyedServices(nameof(GlobalGameContext))] IGameContext gameContext,
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
