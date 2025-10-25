using Microsoft.Extensions.DependencyInjection;
using TireOcr.Ocr.Application.Repositories;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Infrastructure.Repositories;
using TireOcr.Ocr.Infrastructure.Services;
using TireOcr.Ocr.Infrastructure.Services.ImageUtils;
using TireOcr.Ocr.Infrastructure.Services.TireCodeDetectorResolver;

namespace TireOcr.Ocr.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        AddClients(services);
        AddRepositories(services);
        AddServices(services);
        return services;
    }

    private static void AddClients(IServiceCollection services)
    {
        services.AddHttpClient<ITireCodeDetectorResolverService>(c => c.Timeout = TimeSpan.FromSeconds(30));
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IPromptRepository, PromptRepositoryConfiguration>();
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddSingleton<IImageConvertorService, ImageConvertorService>();

        services.AddScoped<ITireCodeDetectorResolverService, TireCodeDetectorResolverService>();
        services.AddScoped<ITireCodeOcrService, TireCodeOcrService>();
        services.AddScoped<ICostEstimationService, ConfigurationCostEstimationService>();
        services.AddScoped<IImageResizeService, ImageResizeService>();
    }
}