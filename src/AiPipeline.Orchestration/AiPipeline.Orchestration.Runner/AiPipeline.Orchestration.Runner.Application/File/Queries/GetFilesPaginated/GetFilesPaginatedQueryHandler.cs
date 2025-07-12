using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.File.Dtos;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.File.Queries.GetFilesPaginated;

public class GetFilesPaginatedQueryHandler : IQueryHandler<GetFilesPaginatedQuery, PaginatedCollection<GetFileDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetFilesPaginatedQueryHandler> _logger;

    public GetFilesPaginatedQueryHandler(IUnitOfWork unitOfWork,
        ILogger<GetFilesPaginatedQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<DataResult<PaginatedCollection<GetFileDto>>> Handle(
        GetFilesPaginatedQuery request,
        CancellationToken cancellationToken
    )
    {
        var foundFiles =
            await _unitOfWork
                .FileRepository
                .GetFilesPaginatedAsync(request.Pagination,
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