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

            var main = Register.ServiceProvider.GetRequiredKeyedService<IGameContext>(
                nameof(MainGameContext)
            );
            if (main.IsNext)
            {
                Console.WriteLine("可以执行下一步");
            }
            else
            {
                await main.InitGameSettingsAsync();
            }
            await main.SaveConfigAsync(
                GameLocalSettingName.GameLauncherBassFolder,
                "D:\\Wuthering Waves\\Wuthering Waves Game"
            );
            var result = await main.ReadConfigAsync(GameLocalSettingName.GameLauncherBassFolder);
            Console.WriteLine("配置参数不完整！");
        }
    }
}
