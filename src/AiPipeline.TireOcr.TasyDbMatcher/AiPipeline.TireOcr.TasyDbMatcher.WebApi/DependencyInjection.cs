using Asp.Versioning;

namespace AiPipeline.TireOcr.TasyDbMatcher.WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        AddControllersConfig(services);
        AddRoutesConfig(services);
        AddSwagger(services);
        return services;
    }

    private static void AddControllersConfig(IServiceCollection serviceCollection)
    {
        serviceCollection.AddControllers();
    }

    private static void AddRoutesConfig(IServiceCollection serviceCollection)
    {
        serviceCollection.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });
    }

    private static void AddSwagger(IServiceCollection serviceCollection)
    {
        serviceCollection.AddEndpointsApiExplorer();
        serviceCollection.AddSwaggerGen();
    }
}