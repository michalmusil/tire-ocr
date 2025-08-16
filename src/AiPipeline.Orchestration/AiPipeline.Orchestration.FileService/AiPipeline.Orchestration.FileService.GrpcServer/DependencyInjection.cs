using AiPipeline.Orchestration.FileService.GrpcServer.Constants;

namespace AiPipeline.Orchestration.FileService.GrpcServer;

public static class DependencyInjection
{

    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddGrpc(opts =>
        {
            opts.MaxReceiveMessageSize = GrpcServerConstants.MaxReceiveMessageSize;
            opts.MaxSendMessageSize = GrpcServerConstants.MaxSendMessageSize;
        });

        return services;
    }
}