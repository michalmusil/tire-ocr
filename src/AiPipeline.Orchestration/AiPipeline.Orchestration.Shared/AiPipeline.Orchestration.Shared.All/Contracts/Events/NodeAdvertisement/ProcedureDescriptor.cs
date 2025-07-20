using AiPipeline.Orchestration.Shared.All.Contracts.Schema;

namespace AiPipeline.Orchestration.Shared.All.Contracts.Events.NodeAdvertisement;

public record ProcedureDescriptor
{
    public required string Id { get; init; }
    public int SchemaVersion { get; init; } = 1;
    public required IApElement Input { get; init; }
    public required IApElement Output { get; init; }
}