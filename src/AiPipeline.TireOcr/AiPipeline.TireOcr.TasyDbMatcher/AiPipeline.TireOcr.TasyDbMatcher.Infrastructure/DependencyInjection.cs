using AiPipeline.TireOcr.TasyDbMatcher.Application.Repositories;
using AiPipeline.TireOcr.TasyDbMatcher.Application.Services;
using AiPipeline.TireOcr.TasyDbMatcher.Infrastructure.Repositories;
using AiPipeline.TireOcr.TasyDbMatcher.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AiPipeline.TireOcr.TasyDbMatcher.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        AddRepositories(services);
        AddServices(services);
        return services;
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddHttpClient<ITireParamsDbRepository, RemoteTireParamsDbRepository>(client =>
        {
            client.BaseAddress = new("https+http://pneuboss.tasy2.client.puxdesign.cz");
        });
    }
    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<ITireCodeDbMatchingService, LevenshteinTireCodeMatchingService>();
    }
}