namespace WutheringWavesTool.ViewModel.GameViewModels;

public sealed partial class BilibiliGameViewModel : GameViewModelBase
{
    public BilibiliGameViewModel(
        [FromKeyedServices(nameof(BilibiliGameContext))] IGameContext gameContext,
        IPickersService pickersService,
        IAppContext<App> appContext,
        ITipShow tipShow
    )
        : base(gameContext, pickersService, appContext, tipShow) { }
}
