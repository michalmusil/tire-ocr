using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Providers;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Commands.RunPipeline;

public class RunPipelineCommandHandler : ICommandHandler<RunPipelineCommand, PipelineDto>
{
    private readonly IPipelineBuilderProvider _pipelineBuilderProvider;
    private readonly ILogger<RunPipelineCommandHandler> _logger;

    public RunPipelineCommandHandler(IPipelineBuilderProvider pipelineBuilderProvider,
        ILogger<RunPipelineCommandHandler> logger)
    {
        _pipelineBuilderProvider = pipelineBuilderProvider;
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
        pipelineBuilder.AddFiles(request.Dto.InputFiles);

        var pipelineResult = await pipelineBuilder.BuildAsync();

        return pipelineResult.Map(
            onSuccess: pipeline => DataResult<PipelineDto>.Success(PipelineDto.FromDomain(pipeline)),
            onFailure: DataResult<PipelineDto>.Failure
        );
    }
}