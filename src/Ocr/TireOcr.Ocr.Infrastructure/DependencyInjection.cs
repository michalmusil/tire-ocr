using Microsoft.Extensions.DependencyInjection;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Infrastructure.Services;
using TireOcr.Ocr.Infrastructure.Services.ImageUtils;
using TireOcr.Ocr.Infrastructure.Services.TireCodeDetectorResolver;

namespace TireOcr.Ocr.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        AddClients(services);
        AddServices(services);
        return services;
    }

    private static void AddClients(IServiceCollection services)
    {
        services.AddHttpClient<ITireCodeDetectorResolver>(c => c.Timeout = TimeSpan.FromSeconds(30));
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddSingleton<IImageUtils, ImageUtils>();

        services.AddScoped<ITireCodeDetectorResolver, TireCodeDetectorResolver>();
        services.AddScoped<ITireCodeDetectionService, TireCodeDetectionService>();
    }
}