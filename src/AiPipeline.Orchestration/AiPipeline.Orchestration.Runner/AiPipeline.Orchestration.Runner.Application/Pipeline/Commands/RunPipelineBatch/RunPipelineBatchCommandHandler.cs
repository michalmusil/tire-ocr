using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Commands.RunPipelineBatch;

public class RunPipelineBatchCommandHandler: ICommandHandler<RunPipelineBatchCommand, GetPipelineBatchDto>
{
    public Task<DataResult<GetPipelineBatchDto>> Handle(RunPipelineBatchCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}