using AiPipeline.Orchestration.FileService.GrpcSdk.Clients.FileServiceClient;
using AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.DownloadFile;
using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Shared.NodeSdk.Services.FileReferenceDownloader;

public class GrpcFileReferenceDownloaderService : IFileReferenceDownloaderService
{
    private readonly IFileSerivceClient _grpcFileClient;

    public GrpcFileReferenceDownloaderService(IFileSerivceClient grpcFileClient)
    {
        _grpcFileClient = grpcFileClient;
    }

    public async Task<DataResult<Stream>> DownloadFileReferenceDataAsync(FileReference reference, Guid userId)
    {
        var request = new DownloadFileRequest(reference.Id, userId);
        var response = await _grpcFileClient.DownloadFileAsync(request);
        if (response.IsFailure)
            return DataResult<Stream>.Failure(response.Failures);

        return DataResult<Stream>.Success(response.Data!.FileData);
    }
}