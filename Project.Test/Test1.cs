using Microsoft.Extensions.DependencyInjection;
using Waves.Core.GameContext;
using Waves.Core.GameContext.Contexts;

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
        public void TestMethod1()
        {
            var main = Register.ServiceProvider.GetRequiredKeyedService<IGameContext>(
                nameof(MainGameContext)
            );
        }
    }
}
