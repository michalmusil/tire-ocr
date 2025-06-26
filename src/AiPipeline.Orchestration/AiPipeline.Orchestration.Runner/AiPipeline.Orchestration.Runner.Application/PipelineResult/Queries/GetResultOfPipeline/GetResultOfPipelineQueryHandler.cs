using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Repositories;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Queries.GetResultOfPipeline;

public class GetResultOfPipelineQueryHandler : IQueryHandler<GetResultOfPipelineQuery, GetPipelineResultDto>
{
    private readonly IPipelineResultRepository _pipelineResultRepository;
    private readonly ILogger<GetResultOfPipelineQueryHandler> _logger;

    public GetResultOfPipelineQueryHandler(IPipelineResultRepository pipelineResultRepository,
        ILogger<GetResultOfPipelineQueryHandler> logger)
    {
        _pipelineResultRepository = pipelineResultRepository;
        _logger = logger;
    }

    public async Task<DataResult<GetPipelineResultDto>> Handle(
        GetResultOfPipelineQuery request,
        CancellationToken cancellationToken
    )
    {
        var foundResult = await _pipelineResultRepository.GetPipelineResultByIdAsync(request.PipelineId.ToString());
        if (foundResult is null)
            return DataResult<GetPipelineResultDto>.NotFound($"Result for pipeline {request.PipelineId} doesn't exist");
        
        var resultDto = GetPipelineResultDto.FromDomain(foundResult);

        return DataResult<GetPipelineResultDto>.Success(resultDto);
    }
}