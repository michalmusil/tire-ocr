namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos;

public record GetPipelineBatchDto(
    Guid PipelineBatchId,
    Guid UserId,
    DateTime? FinishedAt,
    long TotalPipelineCount,
    long FinishedPipelineCount
);