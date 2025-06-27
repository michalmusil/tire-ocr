namespace AiPipeline.Orchestration.Runner.Application.NodeType.Dtos;

public record SaveNodeDto(
    string NodeId,
    IEnumerable<SaveNodeProcedureDto> Procedures
)
{
    public Domain.NodeTypeAggregate.NodeType ToDomain() => new(
        id: NodeId,
        availableProcedures: Procedures.Select(p => p.ToDomain())
    );
}