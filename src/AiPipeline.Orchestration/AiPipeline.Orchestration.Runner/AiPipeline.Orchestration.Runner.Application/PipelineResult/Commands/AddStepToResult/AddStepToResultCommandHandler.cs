using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Repositories;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Commands.AddStepToResult;

public class AddStepToResultCommandHandler : ICommandHandler<AddStepToResultCommand, GetPipelineResultDto>
{
    private readonly IPipelineResultRepository _pipelineResultRepository;
    private readonly ILogger<AddStepToResultCommandHandler> _logger;

    public AddStepToResultCommandHandler(IPipelineResultRepository pipelineResultRepository,
        ILogger<AddStepToResultCommandHandler> logger)
    {
        _pipelineResultRepository = pipelineResultRepository;
        _logger = logger;
    }

    public async Task<DataResult<GetPipelineResultDto>> Handle(
        AddStepToResultCommand request,
        CancellationToken cancellationToken
    )
    {
        var existingResult =
            await _pipelineResultRepository.GetPipelineResultByPipelineIdAsync(request.PipelineId.ToString());
        if (existingResult is null)
            return DataResult<GetPipelineResultDto>.NotFound($"Result for pipeline {request.PipelineId} not found");

        var newStep = request.Dto.ToDomain(existingResult.Id);
        var addStepResult = existingResult.AddStepResult(newStep);
        if (addStepResult.IsFailure)
            return DataResult<GetPipelineResultDto>.Failure(addStepResult.Failures);

        var validationResult = existingResult.Validate();
        if (validationResult.IsFailure)
            return DataResult<GetPipelineResultDto>.Failure(validationResult.Failures);

        await _pipelineResultRepository.SaveChangesAsync();

        var dto = GetPipelineResultDto.FromDomain(existingResult);
        return DataResult<GetPipelineResultDto>.Success(dto);
    }
}