using AiPipeline.TireOcr.TasyDbMatcher.Application.Repositories;
using AiPipeline.TireOcr.TasyDbMatcher.Application.Services;
using AiPipeline.TireOcr.TasyDbMatcher.Infrastructure.Repositories;
using AiPipeline.TireOcr.TasyDbMatcher.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiPipeline.TireOcr.TasyDbMatcher.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddRepositories(services, configuration);
        AddServices(services);
        return services;
    }

    private static void AddRepositories(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        var remoteDbAddress = configuration.GetValue<string>("RemoteTireDbAddress") ?? "";
        services.AddHttpClient<ITireParamsDbRepository, RemoteTireParamsDbRepository>(client =>
        {
            client.BaseAddress = new(remoteDbAddress);
        });
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<ITireCodeDbMatchingService, LevenshteinTireCodeMatchingService>();
    }
}