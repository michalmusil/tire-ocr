using AiPipeline.Orchestration.Runner.Domain.PipelineAggregate;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos;

public record GetPipelineBatchDto(
    Guid PipelineBatchId,
    Guid UserId,
    long TotalPipelineCount
)
{
    public static GetPipelineBatchDto FromDomain(PipelineBatch domain)
    {
        return new GetPipelineBatchDto(
            PipelineBatchId: domain.Id,
            UserId: domain.UserId,
            TotalPipelineCount: domain.Pipelines.Count
        );
    }
}