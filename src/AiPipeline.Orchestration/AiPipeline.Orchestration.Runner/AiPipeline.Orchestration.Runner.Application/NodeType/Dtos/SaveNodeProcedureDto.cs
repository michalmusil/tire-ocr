using AiPipeline.Orchestration.Runner.Domain.NodeTypeAggregate;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;

namespace AiPipeline.Orchestration.Runner.Application.NodeType.Dtos;

public record SaveNodeProcedureDto(
    string ProcedureId,
    string NodeTypeId,
    int SchemaVersion,
    IApElement InputSchema,
    IApElement OutputSchema
)
{
    public NodeProcedure ToDomain() => new NodeProcedure(
        id: ProcedureId,
        nodeTypeId: NodeTypeId,
        schemaVersion: SchemaVersion,
        inputSchema: InputSchema,
        outputSchema: OutputSchema
    );
}