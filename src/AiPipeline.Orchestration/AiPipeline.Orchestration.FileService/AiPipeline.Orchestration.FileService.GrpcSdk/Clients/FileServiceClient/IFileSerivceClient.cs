using AiPipeline.Orchestration.FileService.GrpcServer;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.FileService.GrpcSdk.Clients.FileServiceClient;

public interface IFileSerivceClient
{
    public Task<DataResult<GetAllFilesResponse>> GetAllFilesPaginatedAsync(GetAllFilesRequest request,
        CancellationToken? ct = null);
}