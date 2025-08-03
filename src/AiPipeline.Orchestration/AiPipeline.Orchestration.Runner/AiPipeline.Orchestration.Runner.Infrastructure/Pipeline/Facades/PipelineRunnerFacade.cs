using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Facades;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Providers;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Services;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Commands.InitPipelineResult;
using MediatR;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Facades;

public class PipelineRunnerFacade : IPipelineRunnerFacade
{
    private readonly IMediator _mediator;
    private readonly IPipelineBuilderProvider _pipelineBuilderProvider;
    private readonly IPipelinePublisherService _pipelinePublisher;

    public PipelineRunnerFacade(IMediator mediator, IPipelineBuilderProvider pipelineBuilderProvider,
        IPipelinePublisherService pipelinePublisher)
    {
        _mediator = mediator;
        _pipelineBuilderProvider = pipelineBuilderProvider;
        _pipelinePublisher = pipelinePublisher;
    }

    public async Task<DataResult<Domain.PipelineAggregate.Pipeline>> RunPipelineAsync(RunPipelineDto runDto)
    {
        var pipelineBuilder = _pipelineBuilderProvider.GetPipelineBuilder();

        pipelineBuilder.SetPipelineInput(runDto.Input);
        pipelineBuilder.AddSteps(runDto.Steps);

        var pipelineBuildResult = await pipelineBuilder.BuildAsync();

        if (pipelineBuildResult.IsFailure)
            return DataResult<Domain.PipelineAggregate.Pipeline>.Failure(pipelineBuildResult.Failures);
        var pipeline = pipelineBuildResult.Data!;

        var pipelinePublishResult = await _pipelinePublisher.PublishAsync(pipeline, runDto.Input);

        return await pipelinePublishResult.MapAsync(
            onSuccess: async () =>
            {
                await _mediator.Send(new InitPipelineResultCommand(pipeline.Id));
                return DataResult<Domain.PipelineAggregate.Pipeline>.Success(pipeline);
            },
            onFailure: failures => Task.FromResult(DataResult<Domain.PipelineAggregate.Pipeline>.Failure(failures))
        );
    }
}