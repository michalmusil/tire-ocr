using AiPipeline.Orchestration.FileService.GrpcSdk.Clients.FileServiceClient;
using AiPipeline.Orchestration.FileService.GrpcServer;
using Microsoft.Extensions.DependencyInjection;

namespace AiPipeline.Orchestration.FileService.GrpcSdk;

public static class DependencyInjection
{
    public static IServiceCollection AddFileServiceSdk(this IServiceCollection services, Uri serverUri)
    {
        services.AddGrpcClient<FileServiceInterface.FileServiceInterfaceClient>(
            client => client.Address = serverUri
        );

        services.AddScoped<IFileSerivceClient, GrpcFileServiceClient>();
        return services;
    }
}