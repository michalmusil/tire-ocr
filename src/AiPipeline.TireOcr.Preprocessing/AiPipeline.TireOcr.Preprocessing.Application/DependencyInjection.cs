using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TireOcr.Preprocessing.Application.Options;
using TireOcr.Shared.Behaviors;

namespace TireOcr.Preprocessing.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        AddOptions(services, configuration);
        AddMediator(services);
        AddValidation(services);
        return services;
    }

    private static void AddOptions(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ImageProcessingOptions>(configuration.GetSection(key: nameof(ImageProcessingOptions)));
    }

    private static void AddMediator(IServiceCollection services)
    {
        services.AddMediatR(c => c.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
    }

    private static void AddValidation(IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);
    }
}