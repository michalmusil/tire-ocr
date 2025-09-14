using Microsoft.Extensions.DependencyInjection;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Infrastructure.Services;
using TireOcr.Ocr.Infrastructure.Services.ImageUtils;
using TireOcr.Ocr.Infrastructure.Services.TireCodeDetectorResolver;
using TireOcr.ServiceDefaults;

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
        services.AddHttpClient<ITireCodeDetectorResolverService, TireCodeDetectorResolverService>()
            .RemoveResilienceHandlers()
            .AddStandardResilienceHandler(opt =>
            {
                var timeout = TimeSpan.FromSeconds(90);
                opt.AttemptTimeout.Timeout = timeout;
                opt.TotalRequestTimeout.Timeout = timeout;
                opt.CircuitBreaker.SamplingDuration = 2 * timeout;
            });
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddSingleton<IImageConvertorService, ImageConvertorService>();
        services.AddScoped<ITireCodeOcrService, TireCodeOcrService>();
        services.AddScoped<ICostEstimationService, ConfigurationCostEstimationService>();
    }
}