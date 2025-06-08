using Asp.Versioning;

namespace AiPipeline.Orchestration.Runner.WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        AddRoutesConfig(services);
        AddSwagger(services);
        return services;
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