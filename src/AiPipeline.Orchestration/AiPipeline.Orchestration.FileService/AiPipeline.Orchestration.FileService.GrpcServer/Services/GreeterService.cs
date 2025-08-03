using Grpc.Core;
using AiPipeline.Orchestration.FileService.GrpcServer;

namespace AiPipeline.Orchestration.FileService.GrpcServer.Services;

public class GreeterService : FileService.FileServiceBase
{
    private readonly ILogger<GreeterService> _logger;

    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }
}