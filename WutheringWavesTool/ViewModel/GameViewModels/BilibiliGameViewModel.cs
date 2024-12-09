using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Waves.Core.GameContext;
using Waves.Core.GameContext.Contexts;
using WutheringWavesTool.Common;
using WutheringWavesTool.Services.Contracts;

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

    public override Task LoadedAfter()
    {
        return base.LoadedAfter();
    }
}
