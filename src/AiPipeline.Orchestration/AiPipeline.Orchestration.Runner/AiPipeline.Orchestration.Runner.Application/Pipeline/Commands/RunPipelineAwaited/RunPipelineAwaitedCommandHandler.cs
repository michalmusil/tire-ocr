using AiPipeline.Orchestration.Runner.Application.Pipeline.Providers;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Services;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Commands.InitPipelineResult;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;
using MediatR;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Commands.RunPipelineAwaited;

public class RunPipelineAwaitedCommandHandler : ICommandHandler<RunPipelineAwaitedCommand, GetPipelineResultDto>
{
    private readonly IMediator _mediator;
    private readonly IPipelineBuilderProvider _pipelineBuilderProvider;
    private readonly IPipelinePublisherService _pipelinePublisher;
    private readonly IPipelineResultSubscriberService _pipelineResultSubscriberService;

    public RunPipelineAwaitedCommandHandler(IMediator mediator, IPipelineBuilderProvider pipelineBuilderProvider,
        IPipelinePublisherService pipelinePublisher, IPipelineResultSubscriberService pipelineResultSubscriberService)
    {
        _mediator = mediator;
        _pipelineBuilderProvider = pipelineBuilderProvider;
        _pipelinePublisher = pipelinePublisher;
        _pipelineResultSubscriberService = pipelineResultSubscriberService;
    }

    public async Task<DataResult<GetPipelineResultDto>> Handle(RunPipelineAwaitedCommand request,
        CancellationToken cancellationToken)
    {
        var pipelineBuilder = _pipelineBuilderProvider.GetPipelineBuilder();

        pipelineBuilder.SetPipelineInput(request.Dto.Input);
        pipelineBuilder.AddSteps(request.Dto.Steps);

        var pipelineBuildResult = await pipelineBuilder.BuildAsync();

        if (pipelineBuildResult.IsFailure)
            return DataResult<GetPipelineResultDto>.Failure(pipelineBuildResult.Failures);
        var pipeline = pipelineBuildResult.Data!;

        var pipelinePublishResult = await _pipelinePublisher.PublishAsync(pipeline, request.Dto.Input);
        if (pipelinePublishResult.IsSuccess)
            await _mediator.Send(new InitPipelineResultCommand(pipeline.Id), cancellationToken);
        else
            return DataResult<GetPipelineResultDto>.Failure(pipelinePublishResult.Failures);

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