using AiPipeline.Orchestration.FileService.GrpcSdk.Extensions;
using AiPipeline.Orchestration.FileService.GrpcServer;
using Grpc.Core;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.FileService.GrpcSdk.Clients.FileServiceClient;

public class GrpcFileServiceClient : IFileSerivceClient
{
    private readonly FileServiceInterface.FileServiceInterfaceClient _client;

    public GrpcFileServiceClient(FileServiceInterface.FileServiceInterfaceClient client)
    {
        _client = client;
    }

    public async Task<DataResult<GetAllFilesResponse>> GetAllFilesPaginatedAsync(GetAllFilesRequest request,
        CancellationToken? ct = null)
    {
        try
        {
            var response =
                await _client.GetAllFilesPaginatedAsync(request, cancellationToken: ct ?? CancellationToken.None);
            return DataResult<GetAllFilesResponse>.Success(response);
        }
        catch (RpcException ex)
        {
            return ex.ToDataResult<GetAllFilesResponse>();
        }
        catch
        {
            return DataResult<GetAllFilesResponse>.Failure(new Failure(500, "Grpc failure: Unexpected"));
        }
    }
}