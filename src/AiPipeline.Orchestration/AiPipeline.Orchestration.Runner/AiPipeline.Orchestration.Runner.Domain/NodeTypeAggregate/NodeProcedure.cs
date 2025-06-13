namespace AiPipeline.Orchestration.Runner.Domain.NodeTypeAggregate;

public class NodeProcedure
{
    public string Id { get; }
    public int SchemaVersion { get; }
    public string InputSchema { get; }
    public string OutputSchema { get; }

    public NodeProcedure(string id, int schemaVersion, string inputSchema, string outputSchema)
    {
        Id = id;
        SchemaVersion = schemaVersion;
        InputSchema = inputSchema;
        OutputSchema = outputSchema;
    }
}