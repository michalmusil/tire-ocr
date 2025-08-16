using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResultBatch.Dtos;

public record GetPipelineResultBatchDto(
    Guid Id,
    Guid UserId,
    DateTime? FinishedAt,
    long TotalPipelineCount,
    long SuccessfullyCompletedPipelineCount,
    long FailedPipelineCount,
    List<GetPipelineResultDto> PipelineResults
)
{
    public static GetPipelineResultBatchDto FromDomain(
        Domain.PipelineResultBatchAggregate.PipelineResultBatch domain,
        List<Domain.PipelineResultAggregate.PipelineResult> pipelineResults
    )
    {
        var totalPipelines = pipelineResults.Count;
        var succeeded = pipelineResults.Count(pr => pr.Succeeded);
        var failed = pipelineResults.Count(pr => pr.Failed);

        return new GetPipelineResultBatchDto(
            Id: domain.Id,
            UserId: domain.UserId,
            FinishedAt: domain.FinishedAt,
            TotalPipelineCount: totalPipelines,
            SuccessfullyCompletedPipelineCount: succeeded,
            FailedPipelineCount: failed,
            PipelineResults: pipelineResults
                .Select(GetPipelineResultDto.FromDomain)
                .ToList()
        );
    }
}