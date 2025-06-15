using AiPipeline.Orchestration.Shared.Contracts.Schema;

namespace AiPipeline.Orchestration.Runner.Domain.NodeTypeAggregate;

public class NodeProcedure
{
    public string Id { get; }
    public int SchemaVersion { get; }
    public IApElement InputSchema { get; }
    public IApElement OutputSchema { get; }

    public NodeProcedure(string id, int schemaVersion, IApElement inputSchema, IApElement outputSchema)
    {
        Id = id;
        SchemaVersion = schemaVersion;
        InputSchema = inputSchema;
        OutputSchema = outputSchema;
    }
}