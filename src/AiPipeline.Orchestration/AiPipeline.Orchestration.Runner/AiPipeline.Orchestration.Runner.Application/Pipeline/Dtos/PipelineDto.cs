namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos;

public record PipelineDto(
    string Id,
    List<PipelineStepDto> Steps
)
{
    public static PipelineDto FromDomain(Domain.PipelineAggregate.Pipeline domain)
    {
        return new PipelineDto(
            Id: domain.Id.ToString(),
            Steps: domain.Steps
                .Select(PipelineStepDto.FromDomain)
                .ToList()
        );
    }
}