using Microsoft.Extensions.DependencyInjection;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Infrastructure.Services;

namespace TireOcr.Preprocessing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        AddServices(services);
        return services;
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddSingleton<IImageManipulationService, OpenCvImageManipulationService>();
        services.AddSingleton<ITireDetectionService, YoloTireDetectionService>();
    }
}