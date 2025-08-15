using AiPipeline.Orchestration.Runner.Application.Pipeline.Facades;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Services;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Commands.RunPipelineAwaited;

public class RunPipelineAwaitedCommandHandler : ICommandHandler<RunPipelineAwaitedCommand, GetPipelineResultDto>
{
    private readonly IPipelineRunnerFacade _pipelineRunnerFacade;
    private readonly IPipelineResultSubscriberService _pipelineResultSubscriberService;

    public RunPipelineAwaitedCommandHandler(IPipelineRunnerFacade pipelineRunnerFacade,
        IPipelineResultSubscriberService pipelineResultSubscriberService)
    {
        _pipelineRunnerFacade = pipelineRunnerFacade;
        _pipelineResultSubscriberService = pipelineResultSubscriberService;
    }

    public async Task<DataResult<GetPipelineResultDto>> Handle(RunPipelineAwaitedCommand request,
        CancellationToken cancellationToken)
    {
        var runPipelineResult = await _pipelineRunnerFacade.RunSinglePipelineAsync(request.Dto);
        if (runPipelineResult.IsFailure)
            return DataResult<GetPipelineResultDto>.Failure(runPipelineResult.Failures);

        var pipeline = runPipelineResult.Data!;

        var pipelineResult = await _pipelineResultSubscriberService.WaitForPipelineResultAsync(
            pipelineId: pipeline.Id,
            timeout: request.Timeout,
            cancellationToken: cancellationToken
        );

        return pipelineResult.Map(
            onSuccess: res => DataResult<GetPipelineResultDto>.Success(GetPipelineResultDto.FromDomain(res)),
            onFailure: DataResult<GetPipelineResultDto>.Failure
        );
    }
}