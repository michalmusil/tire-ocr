using AiPipeline.Orchestration.FileService.Application.File.Queries.GetFilesPaginated;
using Grpc.Core;
using AiPipeline.Orchestration.FileService.GrpcServer.Extensions;
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
        var pagination = new PaginationParams(request.PageNumber, request.PageSize);
        var query = new GetFilesPaginatedQuery(
            Pagination: pagination,
            ScopeFilter: request.StorageScopeFilter.ToFileStorageScope()
        );
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            result.PrimaryFailure!.ThrowAsException();

        var data = result.Data!;
        var dataPagination = data.Pagination;
        var dataItems = data.Items
            .Select(dto => new FileDto
            {
                Id = dto.Id.ToString(),
                ContentType = dto.ContentType,
                FileStorageScope = dto.FileStorageScope.ToStorageScope(),
                Path = dto.Path,
                StorageProvider = dto.StorageProvider
            }).ToList();


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
}