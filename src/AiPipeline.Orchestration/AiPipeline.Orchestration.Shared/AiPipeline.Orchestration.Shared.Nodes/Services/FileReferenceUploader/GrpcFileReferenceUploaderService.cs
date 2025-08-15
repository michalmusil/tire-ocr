using AiPipeline.Orchestration.FileService.Domain.FileAggregate;
using AiPipeline.Orchestration.FileService.GrpcSdk.Clients.FileServiceClient;
using AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.UploadFile;
using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.Nodes.Dtos.FileReferenceUploader;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Shared.Nodes.Services.FileReferenceUploader;

public class GrpcFileReferenceUploaderService : IFileReferenceUploaderService
{
    private readonly IFileSerivceClient _grpcClient;

    public GrpcFileReferenceUploaderService(IFileSerivceClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    public async Task<DataResult<FileReference>> UploadFileDataAsync(UploadFileParams input)
    {
        var storageScope = input.StorePermanently ? FileStorageScope.LongTerm : FileStorageScope.Temporary;
        var request = new UploadFileRequest(
            FileName: input.FileName,
            UserId: input.UserId,
            ContentType: input.ContentType,
            FileData: input.FileStream,
            Id: input.FileId,
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