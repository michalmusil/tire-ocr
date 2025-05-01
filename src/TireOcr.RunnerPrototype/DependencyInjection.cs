using System.Text.Json.Serialization;
using TireOcr.RunnerPrototype.Clients;
using TireOcr.RunnerPrototype.Services.CostEstimation;
using TireOcr.RunnerPrototype.Services.TireOcr;

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
        serviceCollection.AddSwaggerGen();
    }

    private static void AddClients(IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient<PreprocessingClient>(client =>
        {
            client.BaseAddress = new("https+http://PreprocessingService");
        });
        serviceCollection.AddHttpClient<OcrClient>(client => { client.BaseAddress = new("https+http://OcrService"); });
        serviceCollection.AddHttpClient<PostprocessingClient>(client =>
        {
            client.BaseAddress = new("https+http://PostprocessingService");
        });
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<ITireOcrService, TireOcrService>();
        services.AddScoped<ICostEstimationService, StaticCostEstimationService>();
    }
}