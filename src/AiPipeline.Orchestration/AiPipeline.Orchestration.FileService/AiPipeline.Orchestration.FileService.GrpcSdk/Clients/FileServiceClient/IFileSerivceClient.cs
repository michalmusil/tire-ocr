using AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.DownloadFile;
using AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.GetAllFiles;
using AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.GetFileById;
using AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.GetFilesByIds;
using AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.UploadFile;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.FileService.GrpcSdk.Clients.FileServiceClient;

public interface IFileSerivceClient
{
    public Task<DataResult<GetAllFilesResponse>> GetAllFilesPaginatedAsync(GetAllFilesRequest request,
        CancellationToken? ct = null);

    public Task<DataResult<GetFileByIdResponse>> GetFileByIdAsync(GetFileByIdRequest request,
        CancellationToken? ct = null);

    public Task<DataResult<GetFilesByIdsResponse>> GetFilesByIdsAsync(GetFilesByIdsRequest request,
        CancellationToken? ct = null);

    public Task<DataResult<UploadFileResponse>> UploadFileAsync(UploadFileRequest request,
        CancellationToken? ct = null);

    public Task<DataResult<DownloadFileResponse>> DownloadFileAsync(DownloadFileRequest request,
        CancellationToken? ct = null);
}