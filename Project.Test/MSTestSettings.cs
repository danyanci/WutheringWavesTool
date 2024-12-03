using Microsoft.Extensions.DependencyInjection;
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
        ServiceProvider = new ServiceCollection()
            .AddTransient<IHttpClientService, HttpClientService>()
            .AddKeyedSingleton<IGameContext, MainGameContext>(
                nameof(MainGameContext),
                (provider, c) =>
                {
                    var context = GameContextFactory.GetMainGameContext();
                    context.HttpClientService = provider.GetRequiredService<IHttpClientService>();
                    context.Init();
                    return context;
                }
            )
            .AddHttpClient()
            .BuildServiceProvider();
    }
}
