using AiPipeline.Orchestration.FileService.Application.File.Dtos;
using AiPipeline.Orchestration.FileService.Domain.FileAggregate;
using AiPipeline.Orchestration.FileService.GrpcSdk.Extensions;
using AiPipeline.Orchestration.FileService.GrpcServer;
using Google.Protobuf;
using Grpc.Core;
using TireOcr.Shared.Result;
using DownloadFileRequest =
    AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.DownloadFile.DownloadFileRequest;
using DownloadFileResponse =
    AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.DownloadFile.DownloadFileResponse;
using GetAllFilesRequest = AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.GetAllFiles.GetAllFilesRequest;
using GetAllFilesResponse =
    AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.GetAllFiles.GetAllFilesResponse;
using GetFileByIdRequest = AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.GetFileById.GetFileByIdRequest;
using GetFileByIdResponse =
    AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.GetFileById.GetFileByIdResponse;
using GetFilesByIdsRequest =
    AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.GetFilesByIds.GetFilesByIdsRequest;
using GetFilesByIdsResponse =
    AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.GetFilesByIds.GetFilesByIdsResponse;
using RemoveFileRequest = AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.RemoveFile.RemoveFileRequest;
using UploadFileRequest = AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.UploadFile.UploadFileRequest;
using UploadFileResponse = AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.UploadFile.UploadFileResponse;

namespace AiPipeline.Orchestration.FileService.GrpcSdk.Clients.FileServiceClient;

public class GrpcFileServiceClient : IFileSerivceClient
{
    private readonly FileServiceInterface.FileServiceInterfaceClient _client;
    private readonly Failure _defaultRpcFailure = new(500, "Unexpected grpc client failure");

    public GrpcFileServiceClient(FileServiceInterface.FileServiceInterfaceClient client)
    {
        _client = client;
    }

    public async Task<DataResult<GetAllFilesResponse>> GetAllFilesPaginatedAsync(GetAllFilesRequest request,
        CancellationToken? ct = null)
    {
        var serverRequest = new GrpcServer.GetAllFilesRequest
        {
            PageNumber = request.PageNumber,
            UserGuid = request.UserId.ToString(),
            PageSize = request.PageSize,
            StorageScopeFilter = GetStorageScope(request.StorageScopeFilter),
        };
        try
        {
            var response =
                await _client.GetAllFilesPaginatedAsync(serverRequest, cancellationToken: ct ?? CancellationToken.None);
            var rp = response.Pagination;

            var result = new GetAllFilesResponse
            (
                Items: response.Items.Select(MapToFileDto),
                Pagination: new TireOcr.Shared.Pagination.Pagination(
                    PageNumber: rp.PageNumber,
                    PageSize: rp.PageSize,
                    TotalCount: rp.TotalCount,
                    TotalPages: rp.TotalPages,
                    HasNextPage: rp.HasNextPage,
                    HasPreviousPage: rp.HasPreviousPage
                )
            );
            return DataResult<GetAllFilesResponse>.Success(result);
        }
        catch (RpcException ex)
        {
            return ex.ToDataResult<GetAllFilesResponse>();
        }
        catch
        {
            return DataResult<GetAllFilesResponse>.Failure(_defaultRpcFailure);
        }
    }

    public async Task<DataResult<GetFileByIdResponse>> GetFileByIdAsync(GetFileByIdRequest request,
        CancellationToken? ct = null)
    {
        var serverRequest = new GrpcServer.GetFileByIdRequest
        {
            FileGuid = request.Id.ToString(),
            UserGuid = request.UserId.ToString(),
        };
        try
        {
            var response =
                await _client.GetFileByIdAsync(serverRequest, cancellationToken: ct ?? CancellationToken.None);
            var result = new GetFileByIdResponse(MapToFileDto(response.File));

            return DataResult<GetFileByIdResponse>.Success(result);
        }
        catch (RpcException ex)
        {
            return ex.ToDataResult<GetFileByIdResponse>();
        }
        catch
        {
            return DataResult<GetFileByIdResponse>.Failure(_defaultRpcFailure);
        }
    }

    public async Task<DataResult<GetFilesByIdsResponse>> GetFilesByIdsAsync(GetFilesByIdsRequest request,
        CancellationToken? ct = null)
    {
        var guidsAsString = request.Ids.Select(id => id.ToString());
        var serverRequest = new GrpcServer.GetFilesByIdsRequest
        {
            UserGuid = request.UserId.ToString(),
            FailIfNotAllFound = request.FailIfNotAllFound,
        };
        serverRequest.FileGuids.AddRange(guidsAsString);

        try
        {
            var response =
                await _client.GetFilesByIdsAsync(serverRequest, cancellationToken: ct ?? CancellationToken.None);

            var result = new GetFilesByIdsResponse
            (
                Items: response.Items.Select(MapToFileDto)
            );
            return DataResult<GetFilesByIdsResponse>.Success(result);
        }
        catch (RpcException ex)
        {
            return ex.ToDataResult<GetFilesByIdsResponse>();
        }
        catch
        {
            return DataResult<GetFilesByIdsResponse>.Failure(_defaultRpcFailure);
        }
    }

    public async Task<DataResult<UploadFileResponse>> UploadFileAsync(UploadFileRequest request,
        CancellationToken? ct = null)
    {
        var serverFileData = await ByteString.FromStreamAsync(request.FileData);
        var serverRequest = new GrpcServer.UploadFileRequest
        {
            FileGuid = request.Id?.ToString(),
            UserGuid = request.UserId.ToString(),
            ContentType = request.ContentType,
            FileName = request.FileName,
            FileData = serverFileData,
            StorageScope = GetStorageScope(request.FileStorageScope)
        };
        try
        {
            var response =
                await _client.UploadFileAsync(serverRequest, cancellationToken: ct ?? CancellationToken.None);
            var result = new UploadFileResponse(MapToFileDto(response.File));

            return DataResult<UploadFileResponse>.Success(result);
        }
        catch (RpcException ex)
        {
            return ex.ToDataResult<UploadFileResponse>();
        }
        catch
        {
            return DataResult<UploadFileResponse>.Failure(_defaultRpcFailure);
        }
    }

    public async Task<DataResult<DownloadFileResponse>> DownloadFileAsync(DownloadFileRequest request,
        CancellationToken? ct = null)
    {
        var serverRequest = new GrpcServer.DownloadFileRequest
        {
            FileGuid = request.Id.ToString(),
            UserGuid = request.UserId.ToString(),
        };
        try
        {
            var response =
                await _client.DownloadFileAsync(serverRequest, cancellationToken: ct ?? CancellationToken.None);

            var dataStream = new MemoryStream(response.FileData.ToByteArray());
            dataStream.Position = 0;
            var result = new DownloadFileResponse(
                ContentType: response.ContentType,
                FileData: dataStream
            );

            return DataResult<DownloadFileResponse>.Success(result);
        }
        catch (RpcException ex)
        {
            return ex.ToDataResult<DownloadFileResponse>();
        }
        catch
        {
            return DataResult<DownloadFileResponse>.Failure(_defaultRpcFailure);
        }
    }

    public async Task<Result> RemoveFileAsync(RemoveFileRequest request, CancellationToken? ct = null)
    {
        var serverRequest = new GrpcServer.RemoveFileRequest
        {
            FileGuid = request.Id.ToString(),
            UserGuid = request.UserId.ToString(),
        };
        try
        {
            var response =
                await _client.RemoveFileAsync(serverRequest, cancellationToken: ct ?? CancellationToken.None);

            if (!response.Success)
                return Result.Failure(new Failure(500, $"Failed to remove file '{request.Id}'"));

            return Result.Success();
        }
        catch (RpcException ex)
        {
            return ex.ToResult();
        }
        catch
        {
            return Result.Failure(_defaultRpcFailure);
        }
    }

    private StorageScope GetStorageScope(FileStorageScope? storageScope)
    {
        return storageScope switch
        {
            FileStorageScope.LongTerm => StorageScope.LongTerm,
            FileStorageScope.ShortTerm => StorageScope.ShortTerm,
            FileStorageScope.Temporary => StorageScope.Temporary,
            _ => StorageScope.FileStorageScopeUnspecified
        };
    }

    private FileStorageScope? GetFileStorageScope(StorageScope storageScope)
    {
        return storageScope switch
        {
            StorageScope.LongTerm => FileStorageScope.LongTerm,
            StorageScope.ShortTerm => FileStorageScope.ShortTerm,
            StorageScope.Temporary => FileStorageScope.Temporary,
            _ => null
        };
    }

    private GetFileDto MapToFileDto(FileDto remoteFileDto)
    {
        return new GetFileDto(
            Id: new Guid(remoteFileDto.FileGuid),
            UserId: new Guid(remoteFileDto.UserGuid),
            FileStorageScope: GetFileStorageScope(remoteFileDto.FileStorageScope) ?? FileStorageScope.Temporary,
            ContentType: remoteFileDto.ContentType,
            StorageProvider: remoteFileDto.StorageProvider,
            Path: remoteFileDto.Path
        );
    }
}