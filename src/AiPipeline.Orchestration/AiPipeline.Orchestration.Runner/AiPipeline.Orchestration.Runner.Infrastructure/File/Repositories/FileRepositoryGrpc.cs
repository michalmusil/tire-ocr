using AiPipeline.Orchestration.FileService.GrpcSdk.Clients.FileServiceClient;
using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using AiPipeline.Orchestration.Runner.Domain.FileAggregate;
using AiPipeline.Orchestration.Runner.Infrastructure.File.Extensions;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.Result;
using DownloadFileRequest =
    AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.DownloadFile.DownloadFileRequest;
using GetAllFilesRequest = AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.GetAllFiles.GetAllFilesRequest;
using GetFileByIdRequest = AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.GetFileById.GetFileByIdRequest;
using GetFilesByIdsRequest =
    AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.GetFilesByIds.GetFilesByIdsRequest;
using RemoveFileRequest = AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.RemoveFile.RemoveFileRequest;
using UploadFileRequest = AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.UploadFile.UploadFileRequest;

namespace AiPipeline.Orchestration.Runner.Infrastructure.File.Repositories;

public class FileRepositoryGrpc : IFileRepository
{
    private readonly IFileSerivceClient _grpcClient;

    public FileRepositoryGrpc(IFileSerivceClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    public async Task<PaginatedCollection<FileValueObject>> GetFilesPaginatedAsync(
        PaginationParams pagination,
        Guid userId,
        FileStorageScope? storageScope = null)
    {
        var serverStorageScope = storageScope.ToGrpcServerStorageScope();
        var request = new GetAllFilesRequest(
            serverStorageScope,
            userId,
            PageNumber: pagination.PageNumber,
            PageSize: pagination.PageSize
        );
        var result = await _grpcClient.GetAllFilesPaginatedAsync(request);
        if (result.IsFailure)
            result.PrimaryFailure!.ThrowAsException();

        var data = result.Data!;
        var valueObjects = data.Items
            .Select(MapGrpcFileToLocalValueObject)
            .ToList();
        var collection = new PaginatedCollection<FileValueObject>(
            items: valueObjects,
            pageNumber: data.Pagination.PageNumber,
            pageSize: data.Pagination.PageSize,
            totalCount: data.Pagination.TotalCount
        );
        return collection;
    }

    public async Task<DataResult<IEnumerable<FileValueObject>>> GetFilesByIdsAsync(Guid userId, params Guid[] fileIds)
    {
        var request = new GetFilesByIdsRequest(fileIds, userId, true);
        var result = await _grpcClient.GetFilesByIdsAsync(request);
        if (result.IsFailure)
            return DataResult<IEnumerable<FileValueObject>>.Failure(result.Failures);

        var valueObjects = result.Data!.Items.Select(MapGrpcFileToLocalValueObject);
        return DataResult<IEnumerable<FileValueObject>>.Success(valueObjects);
    }

    public async Task<DataResult<FileValueObject>> GetFileByIdAsync(Guid fileId, Guid userId)
    {
        var request = new GetFileByIdRequest(fileId, userId);
        var result = await _grpcClient.GetFileByIdAsync(request);
        if (result.IsFailure)
            return DataResult<FileValueObject>.Failure(result.Failures);

        var valueObject = MapGrpcFileToLocalValueObject(result.Data!.File);
        return DataResult<FileValueObject>.Success(valueObject);
    }

    public async Task<DataResult<Stream>> GetFileDataByIdAsync(Guid fileId, Guid userId)
    {
        var request = new DownloadFileRequest(fileId, userId);
        var result = await _grpcClient.DownloadFileAsync(request);
        if (result.IsFailure)
            return DataResult<Stream>.Failure(result.Failures);

        return DataResult<Stream>.Success(result.Data!.FileData);
    }

    public async Task<DataResult<FileValueObject>> Add(Guid userId, string fileName, string contentType,
        Stream fileStream,
        FileStorageScope? storageScope, Guid? guid)
    {
        var request = new UploadFileRequest(
            FileName: fileName,
            UserId: userId,
            ContentType: contentType,
            FileData: fileStream,
            Id: guid,
            FileStorageScope: storageScope.ToGrpcServerStorageScope() ??
                              FileService.Domain.FileAggregate.FileStorageScope.ShortTerm
        );
        var result = await _grpcClient.UploadFileAsync(request);
        if (result.IsFailure)
            return DataResult<FileValueObject>.Failure(result.Failures);

        var valueObject = MapGrpcFileToLocalValueObject(result.Data!.File);
        return DataResult<FileValueObject>.Success(valueObject);
    }

    public async Task<Result> Remove(Guid fileId, Guid userId)
    {
        var request = new RemoveFileRequest(fileId, userId);
        return await _grpcClient.RemoveFileAsync(request);
    }

    private FileStorageScope MapGrpcFileStorageScope(
        FileService.Domain.FileAggregate.FileStorageScope grpcStorageScope)
    {
        return grpcStorageScope switch
        {
            FileService.Domain.FileAggregate.FileStorageScope.LongTerm => FileStorageScope.LongTerm,
            FileService.Domain.FileAggregate.FileStorageScope.ShortTerm => FileStorageScope.ShortTerm,
            FileService.Domain.FileAggregate.FileStorageScope.Temporary => FileStorageScope.Temporary,
            _ => throw new ArgumentOutOfRangeException(nameof(grpcStorageScope), grpcStorageScope, null)
        };
    }

    private FileValueObject MapGrpcFileToLocalValueObject(FileService.Application.File.Dtos.GetFileDto grpcDto)
    {
        return new FileValueObject
        {
            Id = grpcDto.Id,
            UserId = grpcDto.UserId,
            Path = grpcDto.Path,
            ContentType = grpcDto.ContentType,
            StorageProvider = grpcDto.StorageProvider,
            FileStorageScope = MapGrpcFileStorageScope(grpcDto.FileStorageScope)
        };
    }
}