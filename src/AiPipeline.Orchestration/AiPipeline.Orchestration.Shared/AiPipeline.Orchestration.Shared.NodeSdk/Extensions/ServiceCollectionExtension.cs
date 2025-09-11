using System.Reflection;
using AiPipeline.Orchestration.FileService.GrpcSdk;
using AiPipeline.Orchestration.Shared.NodeSdk.Procedures;
using AiPipeline.Orchestration.Shared.NodeSdk.Procedures.Routing;
using AiPipeline.Orchestration.Shared.NodeSdk.Services.FileReferenceDownloader;
using AiPipeline.Orchestration.Shared.NodeSdk.Services.FileReferenceUploader;
using Microsoft.Extensions.DependencyInjection;

namespace AiPipeline.Orchestration.Shared.NodeSdk.Extensions;

public static class ServiceCollectionExtension
{
    public static void AddProcedureRoutingFromAssemblies(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (assemblies is null || assemblies.Length == 0)
            throw new ArgumentException("Assemblies cannot be null or empty.");

        var procedureTypes = assemblies.SelectMany(a => a.GetTypes())
            .Where(type => typeof(IProcedure).IsAssignableFrom(type) &&
                           !type.IsInterface &&
                           !type.IsAbstract &&
                           !type.ContainsGenericParameters);

        foreach (var procedureType in procedureTypes)
            services.AddTransient(procedureType);

        services.AddScoped<IProcedureRouter, ProcedureRouter>();
    }

    public static void AddFileManipulation(this IServiceCollection services, Uri grpcServerUri)
    {
        AddFileManipulationViaGrpcService(services, grpcServerUri);
    }

    private static void AddFileManipulationViaGrpcService(IServiceCollection services, Uri grpcServerUri)
    {
        services.AddFileServiceSdk(grpcServerUri);

        services.AddScoped<IFileReferenceDownloaderService, GrpcFileReferenceDownloaderService>();
        services.AddScoped<IFileReferenceUploaderService, GrpcFileReferenceUploaderService>();
    }
}