using Microsoft.Extensions.DependencyInjection;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Infrastructure.Services;
using TireOcr.Preprocessing.Infrastructure.Services.ModelDownloader;
using TireOcr.Preprocessing.Infrastructure.Services.ModelResolver;

namespace TireOcr.Preprocessing.Infrastructure;

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
        services.AddHttpClient<IMlModelDownloader>(c => c.Timeout = TimeSpan.FromMinutes(3));
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddSingleton<IImageManipulationService, OpenCvImageManipulationService>();
        services.AddSingleton<ITireDetectionService, YoloTireDetectionService>();
        services.AddSingleton<IImageSlicer, OpenCvImageSlicer>();

        services.AddTransient<IMlModelDownloader, MlModelDownloader>();
        services.AddSingleton<IMlModelResolver, MlModelResolver>();
    }
}