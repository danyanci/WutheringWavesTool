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
    }
}
