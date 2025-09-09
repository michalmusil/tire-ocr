namespace TireOcr.AppHost.CustomIntegrations.Minio;

public sealed class MinioResource : ContainerResource, IResourceWithServiceDiscovery
{
    private const string DefaultRootUser = "minio";
    private const string DefaultRootPassword = "minio-root-pass";

    public const string ApiHttpEndpointName = "api";
    public const string ConsoleHttpEndpointName = "console";

    public string RootUser { get; }
    public string RootPassword { get; }
    public EndpointReference ApiHttpEndpoint { get; }
    public EndpointReference ConsoleHttpEndpoint { get; }
    
    public MinioResource(
        string name,
        string? rootUser = null,
        string? rootPassword = null,
        string? entrypoint = null
    ) :
        base(name, entrypoint)
    {
        RootUser = rootUser ?? DefaultRootUser;
        RootPassword = rootPassword ?? DefaultRootPassword;

        ApiHttpEndpoint = new EndpointReference(this, ApiHttpEndpointName);
        ConsoleHttpEndpoint = new EndpointReference(this, ConsoleHttpEndpointName);
    }
}