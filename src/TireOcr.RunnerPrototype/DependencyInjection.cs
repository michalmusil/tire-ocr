using System.Text.Json.Serialization;

namespace TireOcr.RunnerPrototype;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        AddControllersConfig(services);
        AddSwagger(services);
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
}