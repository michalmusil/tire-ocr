using AiPipeline.Orchestration.Contracts.Schema;

namespace AiPipeline.Orchestration.Contracts.Events.NodeAdvertisement;

public record ProcedureDescriptor
{
    public required string Name { get; init; }
    public int SchemaVersion { get; init; } = 1;
    public required IApElement Input { get; init; }
    public required IApElement Output { get; init; }
}