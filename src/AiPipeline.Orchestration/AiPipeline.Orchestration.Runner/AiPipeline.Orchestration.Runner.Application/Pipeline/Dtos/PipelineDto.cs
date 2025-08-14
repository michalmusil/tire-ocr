namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos;

public record PipelineDto(
    Guid Id,
    Guid OwnerId,
    List<PipelineStepDto> Steps
)
{
    public static PipelineDto FromDomain(Domain.PipelineAggregate.Pipeline domain)
    {
        return new PipelineDto(
            Id: domain.Id,
            OwnerId: domain.UserId,
            Steps: domain.Steps
                .Select(PipelineStepDto.FromDomain)
                .ToList()
        );
    }
}