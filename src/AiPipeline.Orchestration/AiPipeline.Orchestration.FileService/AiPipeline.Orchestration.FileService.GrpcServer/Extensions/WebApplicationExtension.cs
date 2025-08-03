using AiPipeline.Orchestration.FileService.GrpcServer.Services;

namespace AiPipeline.Orchestration.FileService.GrpcServer.Extensions;

public static class WebApplicationExtension
{
    public static WebApplication AddGrpcServices(this WebApplication app)
    {
        app.MapGrpcService<GreeterService>();
        return app;
    }
}