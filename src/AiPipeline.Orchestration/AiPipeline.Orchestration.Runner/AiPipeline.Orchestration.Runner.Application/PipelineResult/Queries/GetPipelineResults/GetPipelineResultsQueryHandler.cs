using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Repositories;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Queries.GetPipelineResults;

public class
    GetPipelineResultsQueryHandler : IQueryHandler<GetPipelineResultsQuery, PaginatedCollection<GetPipelineResultDto>>
{
    private readonly IPipelineResultRepository _pipelineResultRepository;
    private readonly ILogger<GetPipelineResultsQueryHandler> _logger;

    public GetPipelineResultsQueryHandler(IPipelineResultRepository pipelineResultRepository,
        ILogger<GetPipelineResultsQueryHandler> logger)
    {
        _pipelineResultRepository = pipelineResultRepository;
        _logger = logger;
    }

    public async Task<DataResult<PaginatedCollection<GetPipelineResultDto>>> Handle(
        GetPipelineResultsQuery request,
        CancellationToken cancellationToken
    )
    {
        var foundResults = await _pipelineResultRepository.GetPipelineResultsPaginatedAsync(request.Pagination);
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