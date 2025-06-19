namespace AiPipeline.Orchestration.Runner.Application.NodeType.Dtos;

public record NodeDto(
    string Id,
    List<NodeProcedureDto> Procedures
)
{
    public static NodeDto FromDomain(Domain.NodeTypeAggregate.NodeType domain)
    {
        return new NodeDto
        (
            Id: domain.Id,
            Procedures: domain.AvailableProcedures
                .Select(NodeProcedureDto.FromDomain)
                .Where(dto => dto is not null)
                .ToList()!
        );
    }
}