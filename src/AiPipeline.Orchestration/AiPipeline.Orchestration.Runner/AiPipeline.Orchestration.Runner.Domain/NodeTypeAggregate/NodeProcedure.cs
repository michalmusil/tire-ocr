using AiPipeline.Orchestration.Runner.Domain.Common;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;

namespace AiPipeline.Orchestration.Runner.Domain.NodeTypeAggregate;

public class NodeProcedure: TimestampedEntity
{
    public string Id { get; }
    public string NodeTypeId { get; }
    public int SchemaVersion { get; }
    public IApElement InputSchema { get; }
    public IApElement OutputSchema { get; }

    public NodeProcedure(string id, string nodeTypeId, int schemaVersion, IApElement inputSchema,
        IApElement outputSchema)
    {
        Id = id;
        NodeTypeId = nodeTypeId;
        SchemaVersion = schemaVersion;
        InputSchema = inputSchema;
        OutputSchema = outputSchema;
    }
}