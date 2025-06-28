namespace TireOcr.AppHost.CustomIntegrations.Minio;

public static class ResourceBuilderMinioExtension
{
    public static IResourceBuilder<MinioResource> AddMinio(this IDistributedApplicationBuilder builder,
        string name,
        int minioApiPort = 9000,
        int minioConsolePort = 9001,
        string? rootUser = null,
        string? rootPassword = null
    )
    {
        var minioResource = new MinioResource(name, rootUser, rootPassword);

        return builder.AddResource(minioResource)
            .WithImage("quay.io/minio/minio")
            .WithImageRegistry("docker.io")
            .WithImageTag("latest")
            .WithArgs("server", "/data")
            .WithVolume("minio-data-volume", "/data")
            .WithEnvironment("MINIO_ADDRESS", ":9000")
            .WithEnvironment("MINIO_CONSOLE_ADDRESS", ":9001")
            .WithEnvironment("MINIO_ROOT_USER", minioResource.RootUser)
            .WithEnvironment("MINIO_ROOT_PASSWORD", minioResource.RootPassword)
            .WithHttpEndpoint(name: MinioResource.ApiHttpEndpointName, port: minioApiPort, targetPort: 9000)
            .WithHttpEndpoint(name: MinioResource.ConsoleHttpEndpointName, port: minioConsolePort,
                targetPort: 9001);
    }

    public static IResourceBuilder<ProjectResource> WithMinioCredentials(this IResourceBuilder<ProjectResource> builder,
        IResourceBuilder<MinioResource> minio, string? username = null, string? password = null)
    {
        return builder
            .WithEnvironment("MINIO_USERNAME", username ?? minio.Resource.RootUser)
            .WithEnvironment("MINIO_PASSWORD", password ?? minio.Resource.RootPassword);
    }
}