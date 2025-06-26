namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;

public record GetPipelineResultDto(
    Guid Id,
    Guid PipelineId,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? FinishedAt,
    IEnumerable<GetPipelineStepResultDto> StepResults
)
{
    public static GetPipelineResultDto FromDomain(Domain.PipelineResultAggregate.PipelineResult domain)
    {
        return new GetPipelineResultDto(
            Id: domain.Id,
            PipelineId: domain.PipelineId,
            CreatedAt: domain.CreatedAt,
            UpdatedAt: domain.UpdatedAt,
            FinishedAt: domain.FinishedAt,
            StepResults: domain.StepResults.Select(GetPipelineStepResultDto.FromDomain)
        );
    }
}