using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Repositories;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Commands.InitPipelineResult;

public class InitPipelineResultCommandHandler : ICommandHandler<InitPipelineResultCommand, GetPipelineResultDto>
{
    private readonly IPipelineResultRepository _pipelineResultRepository;
    private readonly ILogger<InitPipelineResultCommandHandler> _logger;

    public InitPipelineResultCommandHandler(IPipelineResultRepository pipelineResultRepository,
        ILogger<InitPipelineResultCommandHandler> logger)
    {
        _pipelineResultRepository = pipelineResultRepository;
        _logger = logger;
    }

    public async Task<DataResult<GetPipelineResultDto>> Handle(
        InitPipelineResultCommand request,
        CancellationToken cancellationToken
    )
    {
        var newResult = new Domain.PipelineResultAggregate.PipelineResult(request.PipelineId);
        var validationResult = newResult.Validate();
        if (validationResult.IsFailure)
            return DataResult<GetPipelineResultDto>.Failure(validationResult.Failures);

        await _pipelineResultRepository.Add(newResult);
        await _pipelineResultRepository.SaveChangesAsync();

        var dto = GetPipelineResultDto.FromDomain(newResult);
        return DataResult<GetPipelineResultDto>.Success(dto);
    }
}