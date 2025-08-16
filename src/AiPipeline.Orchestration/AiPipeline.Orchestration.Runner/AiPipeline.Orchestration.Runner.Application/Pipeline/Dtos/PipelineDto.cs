using AiPipeline.Orchestration.Shared.All.Contracts.Schema;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos;

public record PipelineDto(
    Guid Id,
    Guid OwnerId,
    IApElement Input,
    List<PipelineStepDto> Steps
)
{
    public static PipelineDto FromDomain(Domain.PipelineAggregate.Pipeline domain)
    {
        return new PipelineDto(
            Id: domain.Id,
            Input: domain.Input,
            OwnerId: domain.UserId,
            Steps: domain.Steps
                .Select(PipelineStepDto.FromDomain)
                .ToList()
        );
    }
}