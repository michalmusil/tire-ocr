using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.File.Dtos;
using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.File.Queries.GetFilesPaginated;

public class GetFilesPaginatedQueryHandler : IQueryHandler<GetFilesPaginatedQuery, PaginatedCollection<FileDto>>
{
    private readonly IFileRepository _fileRepository;
    private readonly ILogger<GetFilesPaginatedQueryHandler> _logger;

    public GetFilesPaginatedQueryHandler(IFileRepository fileRepository,
        ILogger<GetFilesPaginatedQueryHandler> logger)
    {
        _fileRepository = fileRepository;
        _logger = logger;
    }

    public async Task<DataResult<PaginatedCollection<FileDto>>> Handle(
        GetFilesPaginatedQuery request,
        CancellationToken cancellationToken
    )
    {
        var foundFiles =
            await _fileRepository
                .GetFilesPaginatedAsync(request.Pagination,
                    storageScope: request.ScopeFilter
                );

        var fileDtos = foundFiles.Items
            .Select(FileDto.FromDomain)
            .ToList();
        var dtosCollection = new PaginatedCollection<FileDto>(
            fileDtos,
            totalCount: foundFiles.Pagination.TotalCount,
            pageNumber: foundFiles.Pagination.PageNumber,
            pageSize: foundFiles.Pagination.PageSize
        );

        return DataResult<PaginatedCollection<FileDto>>.Success(dtosCollection);
    }
}