using AiPipeline.Orchestration.Runner.Domain.NodeTypeAggregate;

namespace AiPipeline.Orchestration.Runner.Application.Dtos.Node;

public record NodeDto(
    String NodeName,
    List<NodeProcedureDto> Procedures
)
{
    public static NodeDto FromDomain(NodeType domain)
    {
        return new NodeDto
        (
            NodeName: domain.Id,
            Procedures: domain.AvailableProcedures
                .Select(NodeProcedureDto.FromDomain)
                .Where(dto => dto is not null)
                .ToList()!
        );
    }
}