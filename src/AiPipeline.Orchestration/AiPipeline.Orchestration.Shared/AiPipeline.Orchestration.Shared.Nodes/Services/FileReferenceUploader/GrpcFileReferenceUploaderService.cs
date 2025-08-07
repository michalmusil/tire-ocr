using AiPipeline.Orchestration.FileService.Domain.FileAggregate;
using AiPipeline.Orchestration.FileService.GrpcSdk.Clients.FileServiceClient;
using AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.UploadFile;
using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Shared.Nodes.Services.FileReferenceUploader;

public class GrpcFileReferenceUploaderService : IFileReferenceUploaderService
{
    private readonly IFileSerivceClient _grpcClient;

    public GrpcFileReferenceUploaderService(IFileSerivceClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    public async Task<DataResult<FileReference>> UploadFileDataAsync(Guid fileId, Stream fileStream, string contentType,
        string fileName, bool storePermanently)
    {
        var storageScope = storePermanently ? FileStorageScope.LongTerm : FileStorageScope.Temporary;
        var request = new UploadFileRequest(
            FileName: fileName,
            ContentType: contentType,
            FileData: fileStream,
            Id: fileId,
            FileStorageScope: storageScope
        );
        var response = await _grpcClient.UploadFileAsync(request);
        if (response.IsFailure)
            return DataResult<FileReference>.Failure(response.Failures);

        var fileDto = response.Data!.File;
        var reference = new FileReference(
            Id: fileDto.Id,
            StorageProvider: fileDto.StorageProvider,
            Path: fileDto.Path,
            ContentType: fileDto.ContentType
        );

        return DataResult<FileReference>.Success(reference);
    }
}