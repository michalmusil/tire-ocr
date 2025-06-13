namespace AiPipeline.Orchestration.Runner.Domain.PipelineAggregate;

public class PipelineStep
{
    public Guid Id { get; }
    public string NodeId { get; }
    public int SchemaVersion { get; }
    public string InputSchema { get; }
    public string OutputSchema { get; }

    public PipelineStep(string nodeId, int schemaVersion, string inputSchema, string outputSchema, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        NodeId = nodeId;
        SchemaVersion = schemaVersion;
        InputSchema = inputSchema;
        OutputSchema = outputSchema;
    }
}