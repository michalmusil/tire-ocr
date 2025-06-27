using AiPipeline.Orchestration.Runner.Domain.NodeTypeAggregate;
using AiPipeline.Orchestration.Shared.Contracts.Schema;

namespace AiPipeline.Orchestration.Runner.Application.NodeType.Dtos;

public record SaveNodeProcedureDto(
    string ProcedureId,
    int SchemaVersion,
    IApElement InputSchema,
    IApElement OutputSchema
)
{
    public NodeProcedure ToDomain() => new NodeProcedure(
        id: ProcedureId,
        schemaVersion: SchemaVersion,
        inputSchema: InputSchema,
        outputSchema: OutputSchema
    );
}