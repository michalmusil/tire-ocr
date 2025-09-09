using Microsoft.Extensions.DependencyInjection;
using TireOcr.Postprocessing.Application.Services;
using TireOcr.Postprocessing.Infrastructure.Services;

namespace TireOcr.Postprocessing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        AddServices(services);
        return services;
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<ICodeFeatureExtractionService, CodeFeatureExtractionService>();
    }
}