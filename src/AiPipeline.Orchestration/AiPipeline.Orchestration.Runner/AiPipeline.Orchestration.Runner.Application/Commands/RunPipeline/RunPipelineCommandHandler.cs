using AiPipeline.Orchestration.Runner.Application.Dtos.Pipeline;
using AiPipeline.Orchestration.Runner.Application.Services;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.Commands.RunPipeline;

public class RunPipelineCommandHandler : ICommandHandler<RunPipelineCommand, PipelineDto>
{
    private readonly IPipelineBuilderService _pipelineBuilderService;
    private readonly ILogger<RunPipelineCommandHandler> _logger;

    public RunPipelineCommandHandler(IPipelineBuilderService pipelineBuilderService,
        ILogger<RunPipelineCommandHandler> logger)
    {
        _pipelineBuilderService = pipelineBuilderService;
        _logger = logger;
    }

    public async Task<DataResult<PipelineDto>> Handle(
        RunPipelineCommand request,
        CancellationToken cancellationToken
    )
    {
        _pipelineBuilderService.SetPipelineInput(request.Dto.Input);
        _pipelineBuilderService.AddSteps(request.Dto.Steps);
        _pipelineBuilderService.AddFiles(request.Dto.InputFiles);

        var pipelineResult = await _pipelineBuilderService.BuildAsync();

        return pipelineResult.Map(
            onSuccess: pipeline => DataResult<PipelineDto>.Success(PipelineDto.FromDomain(pipeline)),
            onFailure: DataResult<PipelineDto>.Failure
        );
    }
}