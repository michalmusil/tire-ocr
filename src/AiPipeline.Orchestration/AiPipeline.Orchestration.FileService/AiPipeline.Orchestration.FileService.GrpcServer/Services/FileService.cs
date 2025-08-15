using AiPipeline.Orchestration.FileService.Application.File.Commands.RemoveFile;
using AiPipeline.Orchestration.FileService.Application.File.Commands.SaveFile;
using AiPipeline.Orchestration.FileService.Application.File.Dtos;
using AiPipeline.Orchestration.FileService.Application.File.Queries.GetFileById;
using AiPipeline.Orchestration.FileService.Application.File.Queries.GetFilesByIds;
using AiPipeline.Orchestration.FileService.Application.File.Queries.GetFilesPaginated;
using AiPipeline.Orchestration.FileService.Application.File.Queries.GetFileWithDataById;
using AiPipeline.Orchestration.FileService.Domain.FileAggregate;
using Grpc.Core;
using AiPipeline.Orchestration.FileService.GrpcServer.Extensions;
using Google.Protobuf;
using MediatR;
using TireOcr.Shared.Pagination;

namespace AiPipeline.Orchestration.FileService.GrpcServer.Services;

public class FileService : FileServiceInterface.FileServiceInterfaceBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FileService> _logger;

    public FileService(ILogger<FileService> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public override async Task<GetAllFilesResponse> GetAllFilesPaginated(GetAllFilesRequest request,
        ServerCallContext context)
    {
        var userId = new Guid(request.UserGuid);
        var pagination = new PaginationParams(request.PageNumber, request.PageSize);
        var query = new GetFilesPaginatedQuery(
            Pagination: pagination,
            UserId: userId,
            ScopeFilter: request.StorageScopeFilter.ToFileStorageScope()
        );
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            throw result.ToRpcException();

        var data = result.Data!;
        var dataPagination = data.Pagination;
        var dataItems = data.Items
            .Select(MapToFileDto)
            .ToList();


        var response = new GetAllFilesResponse
        {
            Pagination = new Pagination
            {
                HasNextPage = dataPagination.HasNextPage,
                PageNumber = dataPagination.PageNumber,
                PageSize = dataPagination.PageSize,
                TotalCount = dataPagination.TotalCount,
                TotalPages = dataPagination.TotalPages
            }
        };
        response.Items.AddRange(dataItems);

        return response;
    }

    public override async Task<GetFileByIdResponse> GetFileById(GetFileByIdRequest request, ServerCallContext context)
    {
        var userGuid = new Guid(request.UserGuid);
        var fileGuid = new Guid(request.FileGuid);
        var query = new GetFileByIdQuery(fileGuid, userGuid);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            throw result.ToRpcException();

        var fileDtoLocal = result.Data!;
        var fileDtoResponse = MapToFileDto(fileDtoLocal);

        return new GetFileByIdResponse
        {
            File = fileDtoResponse
        };
    }

    public override async Task<GetFilesByIdsResponse> GetFilesByIds(GetFilesByIdsRequest request,
        ServerCallContext context)
    {
        var userGuid = new Guid(request.UserGuid);
        var guids = request.FileGuids
            .Select(fg => new Guid(fg))
            .Where(g => g != Guid.Empty)
            .ToList();
        var query = new GetFilesByIdsQuery(
            FileIds: guids,
            UserId: userGuid,
            FailIfNotAllFound: request.FailIfNotAllFound
        );
        var result = await _mediator.Send(query);
        if (result.IsFailure)
            throw result.ToRpcException();

        var fileDtos = result.Data!
            .Select(MapToFileDto)
            .ToList();

        var response = new GetFilesByIdsResponse();
        response.Items.AddRange(fileDtos);
        return response;
    }

    public override async Task<UploadFileResponse> UploadFile(UploadFileRequest request, ServerCallContext context)
    {
        var userGuid = new Guid(request.UserGuid);
        Guid? fileGuid = request.FileGuid is null ? null : new Guid(request.FileGuid);
        using var fileStream = new MemoryStream(request.FileData.ToByteArray());
        fileStream.Position = 0;

        var command = new SaveFileCommand(
            FileStream: fileStream,
            UserId: userGuid,
            FileStorageScope: request.StorageScope.ToFileStorageScope() ?? FileStorageScope.ShortTerm,
            ContentType: request.ContentType,
            OriginalFileName: request.FileName,
            Id: fileGuid
        );
        var result = await _mediator.Send(command);
        if (result.IsFailure)
            throw result.ToRpcException();

        var fileDtoLocal = result.Data!;
        var fileDtoResponse = MapToFileDto(fileDtoLocal);

        return new UploadFileResponse { File = fileDtoResponse };
    }

    public override async Task<DownloadFileResponse> DownloadFile(DownloadFileRequest request,
        ServerCallContext context)
    {
        var userGuid = new Guid(request.UserGuid);
        var fileGuid = new Guid(request.FileGuid);
        var query = new GetFileWithDataByIdQuery(Id: fileGuid, UserId: userGuid);
        var result = await _mediator.Send(query);
        if (result.IsFailure)
            throw result.ToRpcException();

        var resultDtoLocal = result.Data!;
        var contentType = resultDtoLocal.File.ContentType;

        var fileData = await ByteString.FromStreamAsync(resultDtoLocal.DataStream);
        return new DownloadFileResponse
        {
            ContentType = contentType,
            FileData = fileData
        };
    }

    public override async Task<RemoveFileResponse> RemoveFile(RemoveFileRequest request, ServerCallContext context)
    {
        var userGuid = new Guid(request.UserGuid);
        var fileGuid = new Guid(request.FileGuid);
        var command = new RemoveFileCommand(Id: fileGuid, UserId: userGuid);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return new RemoveFileResponse
            {
                Success = false,
                WasFound = result.PrimaryFailure?.Code != 404
            };

        return new RemoveFileResponse
        {
            Success = true,
            WasFound = true
        };
    }

    private FileDto MapToFileDto(GetFileDto localDto)
    {
        return new FileDto
        {
            FileGuid = localDto.Id.ToString(),
            UserGuid = localDto.UserId.ToString(),
            ContentType = localDto.ContentType,
            FileStorageScope = localDto.FileStorageScope.ToStorageScope(),
            Path = localDto.Path,
            StorageProvider = localDto.StorageProvider
        };
    }
}