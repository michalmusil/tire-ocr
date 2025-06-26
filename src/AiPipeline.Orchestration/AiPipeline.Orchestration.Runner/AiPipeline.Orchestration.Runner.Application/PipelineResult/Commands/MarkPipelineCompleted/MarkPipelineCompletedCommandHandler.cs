using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Repositories;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Commands.MarkPipelineCompleted;

public class MarkPipelineCompletedCommandHandler : ICommandHandler<MarkPipelineCompletedCommand, GetPipelineResultDto>
{
    private readonly IPipelineResultRepository _pipelineResultRepository;
    private readonly ILogger<MarkPipelineCompletedCommandHandler> _logger;

    public MarkPipelineCompletedCommandHandler(IPipelineResultRepository pipelineResultRepository,
        ILogger<MarkPipelineCompletedCommandHandler> logger)
    {
        _pipelineResultRepository = pipelineResultRepository;
        _logger = logger;
    }

    public async Task<DataResult<GetPipelineResultDto>> Handle(
        MarkPipelineCompletedCommand request,
        CancellationToken cancellationToken
    )
    {
        var existingResult =
            await _pipelineResultRepository.GetPipelineResultByPipelineIdAsync(request.PipelineId.ToString());
        if (existingResult is null)
            return DataResult<GetPipelineResultDto>.NotFound($"Result for pipeline {request.PipelineId} not found");

        existingResult.MarkAsFinished(request.CompletedAt);

        var validationResult = existingResult.Validate();
        if (!validationResult.IsFailure)
            return DataResult<GetPipelineResultDto>.Failure(validationResult.Failures);

        await _pipelineResultRepository.SaveChangesAsync();

        var dto = GetPipelineResultDto.FromDomain(existingResult);
        return DataResult<GetPipelineResultDto>.Success(dto);
    }
}