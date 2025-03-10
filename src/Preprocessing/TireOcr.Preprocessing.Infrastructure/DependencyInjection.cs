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
        AddClients(services);
        AddServices(services);
        AddFacades(services);
        return services;
    }

    private static void AddClients(IServiceCollection services)
    {
        services.AddHttpClient<IMlModelDownloader>(c => c.Timeout = TimeSpan.FromMinutes(3));
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddTransient<IMlModelDownloader, MlModelDownloader>();
        services.AddSingleton<IMlModelResolver, MlModelResolver>();

        services.AddScoped<IImageManipulationService, OpenCvImageManipulationService>();
        services.AddScoped<ITireDetectionService, YoloTireDetectionService>();
        services.AddScoped<ITextDetectionService, YoloTextDetectionService>();
        services.AddScoped<IImageSlicer, OpenCvImageSlicer>();
        services.AddScoped<IImageTextApproximator, ImageTextApproximator>();
        services.AddScoped<IContentTypeResolver, ContentTypeResolver>();
    }

    private static void AddFacades(IServiceCollection services)
    {
        services.AddScoped<ITextDetectionFacade, TextDetectionFacade>();
    }
}