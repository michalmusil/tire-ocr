using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TireOcr.Shared.Behaviors;

namespace AiPipeline.TireOcr.TasyDbMatcher.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        AddMediator(services);
        AddValidation(services);
        return services;
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