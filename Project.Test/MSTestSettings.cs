using Microsoft.Extensions.DependencyInjection;
using Waves.Core;
using Waves.Core.Contracts;
using Waves.Core.GameContext;
using Waves.Core.GameContext.Contexts;
using Waves.Core.Services;

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]

public static class Register
{
    public static IServiceProvider ServiceProvider { get; private set; }

    public static void Init()
    {
        ServiceProvider = new ServiceCollection().AddGameContext().BuildServiceProvider();
    }
}
