using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Providers;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Services;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Commands.InitPipelineResult;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Commands.RunPipeline;

public class RunPipelineCommandHandler : ICommandHandler<RunPipelineCommand, PipelineDto>
{
    private readonly IMediator _mediator;
    private readonly IPipelineBuilderProvider _pipelineBuilderProvider;
    private readonly IPipelinePublisherService _pipelinePublisher;
    private readonly ILogger<RunPipelineCommandHandler> _logger;

    public RunPipelineCommandHandler(IMediator mediator, IPipelineBuilderProvider pipelineBuilderProvider,
        IPipelinePublisherService pipelinePublisher, ILogger<RunPipelineCommandHandler> logger)
    {
        _mediator = mediator;
        _pipelineBuilderProvider = pipelineBuilderProvider;
        _pipelinePublisher = pipelinePublisher;
        _logger = logger;
    }

    public async Task<DataResult<PipelineDto>> Handle(
        RunPipelineCommand request,
        CancellationToken cancellationToken
    )
    {
        var pipelineBuilder = _pipelineBuilderProvider.GetPipelineBuilder();

        pipelineBuilder.SetPipelineInput(request.Dto.Input);
        pipelineBuilder.AddSteps(request.Dto.Steps);

        var pipelineResult = await pipelineBuilder.BuildAsync();

        if (pipelineResult.IsFailure)
            return DataResult<PipelineDto>.Failure(pipelineResult.Failures);
        var pipeline = pipelineResult.Data!;

        var pipelinePublishResult = await _pipelinePublisher.PublishPipeline(pipeline, request.Dto.Input);

        return await pipelinePublishResult.MapAsync(
            onSuccess: async () =>
            {
                await _mediator.Send(new InitPipelineResultCommand(pipeline.Id), cancellationToken);
                return DataResult<PipelineDto>.Success(PipelineDto.FromDomain(pipeline));
            },
            onFailure: failures => Task.FromResult(DataResult<PipelineDto>.Failure(failures))
        );
    }
}