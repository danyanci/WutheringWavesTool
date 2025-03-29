using WutheringWavesTool.Services.DialogServices;

namespace WutheringWavesTool.ViewModel.GameViewModels;

public sealed partial class MainGameViewModel : GameContextViewModelBase
{
    public MainGameViewModel(
        [FromKeyedServices(nameof(MainGameContext))] IGameContext gameContext,
        [FromKeyedServices(nameof(MainDialogService))] IDialogManager dialogManager,
        IAppContext<App> appContext
    )
        : base(gameContext, dialogManager, appContext) { }

    public override Task LoadAfter()
    {
        return Task.CompletedTask;
    }
}
