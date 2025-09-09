using AiPipeline.Orchestration.Shared.All.Contracts.Schema;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;

public record GetPipelineResultDto(
    Guid Id,
    Guid UserId,
    Guid PipelineId,
    IApElement? InitialInput,
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
            UserId: domain.UserId,
            PipelineId: domain.PipelineId,
            InitialInput: domain.InitialInput,
            CreatedAt: domain.CreatedAt,
            UpdatedAt: domain.UpdatedAt,
            FinishedAt: domain.FinishedAt,
            StepResults: domain.StepResults.Select(GetPipelineStepResultDto.FromDomain)
        );
    }
}