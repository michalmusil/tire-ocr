using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Facades;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Commands.RunPipeline;

public class RunPipelineCommandHandler : ICommandHandler<RunPipelineCommand, PipelineDto>
{
    private readonly IPipelineRunnerFacade _pipelineRunnerFacade;
    private readonly ILogger<RunPipelineCommandHandler> _logger;

    public RunPipelineCommandHandler(IPipelineRunnerFacade pipelineRunnerFacade,
        ILogger<RunPipelineCommandHandler> logger)
    {
        _pipelineRunnerFacade = pipelineRunnerFacade;
        _logger = logger;
    }

    public async Task<DataResult<PipelineDto>> Handle(
        RunPipelineCommand request,
        CancellationToken cancellationToken
    )
    {
        var runPipelineResult = await _pipelineRunnerFacade.RunSinglePipelineAsync(request.Dto);

        return runPipelineResult.Map(
            onSuccess: pipeline => DataResult<PipelineDto>.Success(PipelineDto.FromDomain(pipeline)),
            onFailure: DataResult<PipelineDto>.Failure
        );
    }
}