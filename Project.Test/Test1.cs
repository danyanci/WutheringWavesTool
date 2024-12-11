using Microsoft.Extensions.DependencyInjection;
using Waves.Core.GameContext;
using Waves.Core.GameContext.Contexts;
using Waves.Core.Models;

namespace Project.Test
{
    [TestClass]
    public sealed class Test1
    {
        public Test1()
        {
            Register.Init();
        }

        [TestMethod]
        public async Task TestMethod1()
        {
            GameContextFactory.GameBassPath = "C:\\Users\\30140\\Documents\\Waves";
            var bilibili = Register.ServiceProvider.GetRequiredKeyedService<IGameContext>(
                nameof(MainGameContext)
            );
            var heander = await bilibili.GetGameLauncherSourceAsync();
            var show = await bilibili.GetGameLauncherStarterAsync(heander, true);
        }
    }
}
