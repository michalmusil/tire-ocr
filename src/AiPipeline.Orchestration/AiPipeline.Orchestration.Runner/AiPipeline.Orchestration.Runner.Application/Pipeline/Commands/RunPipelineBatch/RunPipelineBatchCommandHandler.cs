using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Facades;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Commands.RunPipelineBatch;

public class RunPipelineBatchCommandHandler : ICommandHandler<RunPipelineBatchCommand, GetPipelineBatchDto>
{
    private readonly IPipelineRunnerFacade _pipelineRunnerFacade;

    public RunPipelineBatchCommandHandler(IPipelineRunnerFacade pipelineRunnerFacade)
    {
        _pipelineRunnerFacade = pipelineRunnerFacade;
    }

    public async Task<DataResult<GetPipelineBatchDto>> Handle(RunPipelineBatchCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _pipelineRunnerFacade.RunPipelineBatchAsync(request.RunPipelineBatchDto);

        return result.Map(
            onSuccess: batch => DataResult<GetPipelineBatchDto>.Success(GetPipelineBatchDto.FromDomain(batch)),
            onFailure: DataResult<GetPipelineBatchDto>.Failure
        );
    }
}