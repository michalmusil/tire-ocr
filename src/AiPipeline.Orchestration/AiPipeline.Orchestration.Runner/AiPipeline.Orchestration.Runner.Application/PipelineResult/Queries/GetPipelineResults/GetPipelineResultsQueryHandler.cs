using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Queries.GetPipelineResults;

public class
    GetPipelineResultsQueryHandler : IQueryHandler<GetPipelineResultsQuery, PaginatedCollection<GetPipelineResultDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetPipelineResultsQueryHandler> _logger;

    public GetPipelineResultsQueryHandler(IUnitOfWork unitOfWork,
        ILogger<GetPipelineResultsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<DataResult<PaginatedCollection<GetPipelineResultDto>>> Handle(
        GetPipelineResultsQuery request,
        CancellationToken cancellationToken
    )
    {
        var foundResults = await _unitOfWork
            .PipelineResultRepository
            .GetPipelineResultsPaginatedAsync(request.Pagination, request.UserId);
        var resultDtos = foundResults.Items
            .Select(GetPipelineResultDto.FromDomain)
            .ToList();
        var dtosCollection = new PaginatedCollection<GetPipelineResultDto>(
            resultDtos,
            totalCount: foundResults.Pagination.TotalCount,
            pageNumber: foundResults.Pagination.PageNumber,
            pageSize: foundResults.Pagination.PageSize
        );

        return DataResult<PaginatedCollection<GetPipelineResultDto>>.Success(dtosCollection);
    }
}