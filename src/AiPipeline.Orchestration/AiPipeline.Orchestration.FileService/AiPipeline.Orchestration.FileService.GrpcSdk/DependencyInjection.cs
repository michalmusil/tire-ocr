using AiPipeline.Orchestration.FileService.GrpcSdk.Clients.FileServiceClient;
using AiPipeline.Orchestration.FileService.GrpcSdk.Constants;
using AiPipeline.Orchestration.FileService.GrpcServer;
using Microsoft.Extensions.DependencyInjection;

namespace AiPipeline.Orchestration.FileService.GrpcSdk;

public static class DependencyInjection
{
    public static IServiceCollection AddFileServiceSdk(this IServiceCollection services, Uri serverUri)
    {
        services
            .AddGrpcClient<FileServiceInterface.FileServiceInterfaceClient>(opts => { opts.Address = serverUri; })
            .ConfigureChannel(opts =>
            {
                opts.MaxReceiveMessageSize = GrpcSdkConstants.MaxReceiveMessageSize;
                opts.MaxSendMessageSize = GrpcSdkConstants.MaxSendMessageSize;
            });

        services.AddScoped<IFileSerivceClient, GrpcFileServiceClient>();
        return services;
    }
}