using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Queries.GetResultOfPipeline;

public class GetResultOfPipelineQueryHandler : IQueryHandler<GetResultOfPipelineQuery, GetPipelineResultDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetResultOfPipelineQueryHandler> _logger;

    public GetResultOfPipelineQueryHandler(IUnitOfWork unitOfWork,
        ILogger<GetResultOfPipelineQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<DataResult<GetPipelineResultDto>> Handle(
        GetResultOfPipelineQuery request,
        CancellationToken cancellationToken
    )
    {
        var foundResult = await _unitOfWork
            .PipelineResultRepository
            .GetPipelineResultByPipelineIdAsync(request.PipelineId);
        if (foundResult is null)
            return DataResult<GetPipelineResultDto>.NotFound($"Result for pipeline {request.PipelineId} doesn't exist");

        var resultDto = GetPipelineResultDto.FromDomain(foundResult);

        return DataResult<GetPipelineResultDto>.Success(resultDto);
    }
}