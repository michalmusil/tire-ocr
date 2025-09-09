using AiPipeline.Orchestration.FileService.Application.File.Dtos;
using AiPipeline.Orchestration.FileService.Application.File.Repositories;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.FileService.Application.File.Queries.GetFilesPaginated;

public class GetFilesPaginatedQueryHandler : IQueryHandler<GetFilesPaginatedQuery, PaginatedCollection<GetFileDto>>
{
    private readonly IFileEntityRepository _fileEntityRepository;
    private readonly ILogger<GetFilesPaginatedQueryHandler> _logger;

    public GetFilesPaginatedQueryHandler(IFileEntityRepository fileEntityRepository,
        ILogger<GetFilesPaginatedQueryHandler> logger)
    {
        _fileEntityRepository = fileEntityRepository;
        _logger = logger;
    }

    public async Task<DataResult<PaginatedCollection<GetFileDto>>> Handle(
        GetFilesPaginatedQuery request,
        CancellationToken cancellationToken
    )
    {
        var foundFiles =
            await _fileEntityRepository.GetFilesPaginatedAsync(
                request.Pagination,
                request.UserId,
                storageScope: request.ScopeFilter
            );

        var fileDtos = foundFiles.Items
            .Select(GetFileDto.FromDomain)
            .ToList();
        var dtosCollection = new PaginatedCollection<GetFileDto>(
            fileDtos,
            totalCount: foundFiles.Pagination.TotalCount,
            pageNumber: foundFiles.Pagination.PageNumber,
            pageSize: foundFiles.Pagination.PageSize
        );

        return DataResult<PaginatedCollection<GetFileDto>>.Success(dtosCollection);
    }
}