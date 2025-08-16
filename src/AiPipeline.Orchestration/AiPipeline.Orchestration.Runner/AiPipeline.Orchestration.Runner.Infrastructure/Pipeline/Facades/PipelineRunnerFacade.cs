using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Facades;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Providers;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Services;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Commands.InitPipelineResult;
using AiPipeline.Orchestration.Runner.Application.PipelineResultBatch.Commands.InitPipelineBatchResults;
using AiPipeline.Orchestration.Runner.Domain.PipelineAggregate;
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

    public async Task<DataResult<Domain.PipelineAggregate.Pipeline>> RunSinglePipelineAsync(RunPipelineDto runDto)
    {
        var pipelineBuilder = _pipelineBuilderProvider.GetPipelineBuilder(pipelineOwnerId: runDto.UserId);

        pipelineBuilder.SetPipelineInput(runDto.Input);
        pipelineBuilder.AddSteps(runDto.Steps);

        var pipelineBuildResult = await pipelineBuilder.ValidateAndBuildAsync();

        if (pipelineBuildResult.IsFailure)
            return DataResult<Domain.PipelineAggregate.Pipeline>.Failure(pipelineBuildResult.Failures);
        var pipeline = pipelineBuildResult.Data!;

        var initResult = await _mediator.Send(
            new InitPipelineResultCommand(
                PipelineId: pipeline.Id,
                BatchId: null,
                Input: pipeline.Input,
                UserId: pipeline.UserId
            )
        );
        return await initResult.MapAsync(
            onSuccess: async () =>
            {
                var pipelinePublishResult = await _pipelinePublisher.PublishAsync(pipeline);
                return pipelinePublishResult.Map(
                    onSuccess: () => DataResult<Domain.PipelineAggregate.Pipeline>.Success(pipeline),
                    onFailure: DataResult<Domain.PipelineAggregate.Pipeline>.Failure
                );
            },
            onFailure: failures => Task.FromResult(DataResult<Domain.PipelineAggregate.Pipeline>.Failure(failures))
        );
    }

    public async Task<DataResult<PipelineBatch>> RunPipelineBatchAsync(RunPipelineBatchDto runDto)
    {
        var pipelineBuilder = _pipelineBuilderProvider.GetPipelineBatchBuilder(pipelineOwnerId: runDto.UserId);

        pipelineBuilder.AddInputs(runDto.Inputs.ToArray());
        pipelineBuilder.AddSteps(runDto.Steps.ToArray());

        var pipelineBatchBuildResult = await pipelineBuilder.ValidateAndBuildAsync();

        if (pipelineBatchBuildResult.IsFailure)
            return DataResult<PipelineBatch>.Failure(pipelineBatchBuildResult.Failures);
        var batch = pipelineBatchBuildResult.Data!;

        var initResult = await _mediator.Send(new InitPipelineBatchResultsCommand(batch));
        return await initResult.MapAsync(
            onSuccess: async () =>
            {
                var pipelinePublishResult = await _pipelinePublisher.PublishBatchAsync(batch);
                return pipelinePublishResult.Map(
                    onSuccess: () => DataResult<PipelineBatch>.Success(batch),
                    onFailure: DataResult<PipelineBatch>.Failure
                );
            },
            onFailure: failures => Task.FromResult(DataResult<PipelineBatch>.Failure(failures))
        );
    }
}