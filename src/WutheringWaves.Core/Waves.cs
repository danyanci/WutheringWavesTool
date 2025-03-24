using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using WutheringWaves.Core.Contracts;
using WutheringWaves.Core.GameContext;
using WutheringWaves.Core.Services;

namespace Waves.Core;

public static class Waves
{
    public static IServiceCollection AddGameContext(this IServiceCollection services)
    {
        StaticConfig.EnableAot = true;
        services.AddKeyedSingleton<IGameContext, MainGameContext>(
            nameof(MainGameContext),
            (provider, c) =>
            {
                var context = GameContextFactory.GetMainGameContext();
                context.HttpClientService = provider.GetRequiredService<IHttpClientService>();
                return context;
            }
        );
        services.AddTransient<IHttpClientService, HttpClientService>();
        services.AddHttpClient();
        return services;
    }
}
