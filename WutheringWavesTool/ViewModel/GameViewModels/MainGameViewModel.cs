using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Waves.Core.GameContext;
using Waves.Core.GameContext.Contexts;
using WutheringWavesTool.Services.Contracts;

namespace WutheringWavesTool.ViewModel.GameViewModels;

public sealed partial class MainGameViewModel : GameViewModelBase
{
    public MainGameViewModel(
        [FromKeyedServices(nameof(MainGameContext))] IGameContext gameContext,
        IPickersService pickersService,
        IAppContext<App> appContext
    )
        : base(gameContext, pickersService, appContext) { }

    public override Task LoadedAfter()
    {
        return base.LoadedAfter();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}
