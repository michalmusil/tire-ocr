namespace AiPipeline.Orchestration.FileService.GrpcServer;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddGrpc();

        return services;
    }
}