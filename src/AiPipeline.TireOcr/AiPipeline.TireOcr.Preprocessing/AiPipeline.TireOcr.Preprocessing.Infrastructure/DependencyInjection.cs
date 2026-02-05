using Microsoft.Extensions.DependencyInjection;
using TireOcr.Preprocessing.Application.Facades;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Infrastructure.Facades;
using TireOcr.Preprocessing.Infrastructure.Services;
using TireOcr.Preprocessing.Infrastructure.Services.ModelDownloader;
using TireOcr.Preprocessing.Infrastructure.Services.ModelResolver;

namespace TireOcr.Preprocessing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        AddServices(services);
        AddFacades(services);
        return services;
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddTransient<IMlModelDownloaderService, MlModelDownloaderService>();
        services.AddSingleton<IMlModelResolverService, MlModelResolverService>();

        services.AddScoped<IImageManipulationService, OpenCvImageManipulationService>();
        services.AddScoped<ITireDetectionService, YoloTireDetectionService>();
        services.AddScoped<ITextDetectionService, YoloTextDetectionService>();
        services.AddScoped<IImageSlicerService, OpenCvImageSlicerService>();
        services.AddScoped<IImageTextApproximatorService, ImageTextApproximatorService>();
        services.AddScoped<IContentTypeResolverService, ContentTypeResolverService>();
        services.AddScoped<ITireSidewallExtractionService, TireSidewallExtractionService>();
        services.AddScoped<ICharacterEnhancementService, CharacterEnhancementService>();
    }

    private static void AddFacades(IServiceCollection services)
    {
        services.AddScoped<IRoiExtractionFacade, RoiExtractionFacade>();
    }
}