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
        
        services.AddHttpClient<ITireParamsDbRepository, RemoteTireParamsDbRepository>(client =>
        {
            var tireDbAddress = configuration.GetValue<string>("RemoteTireDbAddress") ?? "";
            client.BaseAddress = new(tireDbAddress);
        });
        services.AddHttpClient<ISupportedManufacturersRepository, SupportedManufacturersRemoteRepository>(client =>
        {
            var manufacturerDbAddress = configuration.GetValue<string>("RemoteManufacturerDbAddress") ?? "";
            client.BaseAddress = new(manufacturerDbAddress);
        });
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<ITireCodeDbMatchingService, LevenshteinTireCodeMatchingService>();
        services.AddScoped<IManufacturerDbMatchingService, ManufacturerDbMatchingService>();
    }
}