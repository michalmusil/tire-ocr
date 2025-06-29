using AiPipeline.Orchestration.Runner.Application.File.Dtos;
using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using AiPipeline.Orchestration.Runner.Application.NodeType.Dtos;
using AiPipeline.Orchestration.Runner.Application.NodeType.Repositories;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.File.Queries.GetFilesPaginated;

public class GetFilesPaginatedQueryHandler : IQueryHandler<GetFilesPaginatedQuery, PaginatedCollection<GetFileDto>>
{
    private readonly IFileRepository _fileRepository;
    private readonly ILogger<GetFilesPaginatedQueryHandler> _logger;

    public GetFilesPaginatedQueryHandler(IFileRepository fileRepository,
        ILogger<GetFilesPaginatedQueryHandler> logger)
    {
        _fileRepository = fileRepository;
        _logger = logger;
    }

    public async Task<DataResult<PaginatedCollection<GetFileDto>>> Handle(
        GetFilesPaginatedQuery request,
        CancellationToken cancellationToken
    )
    {
        var foundFiles =
            await _fileRepository.GetFilesPaginatedAsync(request.Pagination,
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