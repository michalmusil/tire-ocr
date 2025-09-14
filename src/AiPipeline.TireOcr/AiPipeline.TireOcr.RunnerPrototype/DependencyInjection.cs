using System.Reflection;
using System.Text.Json.Serialization;
using TireOcr.RunnerPrototype.Clients.DbMatching;
using TireOcr.RunnerPrototype.Clients.ImageDownload;
using TireOcr.RunnerPrototype.Clients.Ocr;
using TireOcr.RunnerPrototype.Clients.Postprocessing;
using TireOcr.RunnerPrototype.Clients.Preprocessing;
using TireOcr.RunnerPrototype.Services.PipelineRunner;
using TireOcr.ServiceDefaults;

namespace TireOcr.RunnerPrototype;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        AddControllersConfig(services);
        AddSwagger(services);
        AddClients(services);
        AddServices(services);
        return services;
    }

    private static void AddControllersConfig(IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters
                .Add(new JsonStringEnumConverter())
            );
    }

    private static void AddSwagger(IServiceCollection serviceCollection)
    {
        serviceCollection.AddEndpointsApiExplorer();
        serviceCollection.AddSwaggerGen(opt =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }

    private static void AddClients(IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient<IPreprocessingClient, PreprocessingClient>(client =>
            {
                client.BaseAddress = new("https+http://PreprocessingService");
            })
            .RemoveResilienceHandlers()
            .AddStandardResilienceHandler(opt =>
            {
                var timeout = TimeSpan.FromSeconds(90);
                opt.AttemptTimeout.Timeout = timeout;
                opt.TotalRequestTimeout.Timeout = timeout;
                opt.CircuitBreaker.SamplingDuration = 2 * timeout;
            });
        serviceCollection.AddHttpClient<IOcrClient, OcrClient>(client =>
            {
                client.BaseAddress = new("https+http://OcrService");
            })
            .RemoveResilienceHandlers()
            .AddStandardResilienceHandler(opt =>
            {
                var timeout = TimeSpan.FromMinutes(3);
                opt.AttemptTimeout.Timeout = timeout;
                opt.TotalRequestTimeout.Timeout = timeout;
                opt.CircuitBreaker.SamplingDuration = 2 * timeout;
            });
        ;
        serviceCollection.AddHttpClient<IPostprocessingClient, PostprocessingClient>(client =>
        {
            client.BaseAddress = new("https+http://PostprocessingService");
        });
        serviceCollection.AddHttpClient<IDbMatchingClient, DbMatchingClient>(client =>
        {
            client.BaseAddress = new("https+http://TasyDbMatcherService");
        });
        serviceCollection.AddHttpClient<IImageDownloadClient, ImageDownloadClient>();
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<IPipelineRunnerService, PipelineRunnerService>();
    }
}