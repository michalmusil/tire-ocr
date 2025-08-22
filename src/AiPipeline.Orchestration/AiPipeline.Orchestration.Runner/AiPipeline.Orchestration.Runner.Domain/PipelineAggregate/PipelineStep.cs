using AiPipeline.Orchestration.Shared.All.Contracts.Schema;

namespace AiPipeline.Orchestration.Runner.Domain.PipelineAggregate;

public class PipelineStep
{
    public Guid Id { get; }
    public string NodeId { get; }
    public string NodeProcedureId { get; }
    public int SchemaVersion { get; }
    public int Order { get; }
    public string? OutputValueSelector { get; }
    public IApElement InputSchema { get; }
    public IApElement OutputSchema { get; }

    public PipelineStep(string nodeId, string nodeProcedureId, int schemaVersion, int order,
        string? outputValueSelector, IApElement inputSchema, IApElement outputSchema, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        NodeId = nodeId;
        NodeProcedureId = nodeProcedureId;
        SchemaVersion = schemaVersion;
        Order = order;
        OutputValueSelector = outputValueSelector;
        InputSchema = inputSchema;
        OutputSchema = outputSchema;
    }
}