using Asp.Versioning;
using TireOcr.Preprocessing.Infrastructure.Services.ModelDownloader;
using TireOcr.ServiceDefaults;

namespace TireOcr.Preprocessing.WebApi;

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
    
    private static void AddClients(IServiceCollection services)
    {
        services.AddHttpClient<IMlModelDownloaderService>()
            .RemoveResilienceHandlers()
            .AddStandardResilienceHandler(opt =>
            {
                var timeout = TimeSpan.FromMinutes(3);
                opt.AttemptTimeout.Timeout = timeout;
                opt.TotalRequestTimeout.Timeout = timeout;
                opt.CircuitBreaker.SamplingDuration = 2 * timeout;
            });;
    }
}