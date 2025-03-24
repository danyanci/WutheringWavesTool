using Microsoft.Extensions.DependencyInjection;
using Waves.Core;
using WutheringWaves.Core.Contracts;
using WutheringWaves.Core.GameContext;

namespace Project.Test;

[TestClass()]
public class NewCoreTest
{
    IServiceProvider? Provider { get; set; }

    public void InitService()
    {
        Provider = new ServiceCollection().AddGameContext().BuildServiceProvider();
    }

    [TestMethod]
    public void Test1()
    {
        InitService();
        var mainGame = Provider!.GetRequiredKeyedService<IGameContext>(nameof(MainGameContext));
    }
}
