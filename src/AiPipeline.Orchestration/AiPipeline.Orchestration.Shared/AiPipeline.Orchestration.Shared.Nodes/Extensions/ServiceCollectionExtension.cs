using System.Reflection;
using AiPipeline.Orchestration.FileService.GrpcSdk;
using AiPipeline.Orchestration.Shared.Nodes.Procedures;
using AiPipeline.Orchestration.Shared.Nodes.Procedures.Routing;
using AiPipeline.Orchestration.Shared.Nodes.Services.FileReferenceDownloader;
using AiPipeline.Orchestration.Shared.Nodes.Services.FileReferenceUploader;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace AiPipeline.Orchestration.Shared.Nodes.Extensions;

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

    public static void AddFileManipulation(this IServiceCollection services, IConfiguration configuration)
    {
        AddFileManipulationViaGrpcService(services);
    }

    private static void AddFileManipulationViaGrpcService(IServiceCollection services)
    {
        var grpcServerUri = new Uri("http://FileService");
        services.AddFileServiceSdk(grpcServerUri);

        services.AddScoped<IFileReferenceDownloaderService, GrpcFileReferenceDownloaderService>();
        services.AddScoped<IFileReferenceUploaderService, GrpcFileReferenceUploaderService>();
    }
}