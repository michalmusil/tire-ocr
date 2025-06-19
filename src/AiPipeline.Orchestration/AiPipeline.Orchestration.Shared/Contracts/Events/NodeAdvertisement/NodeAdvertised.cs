namespace AiPipeline.Orchestration.Shared.Contracts.Events.NodeAdvertisement;

public record NodeAdvertised
{
    public required string NodeId { get; init; }
    public required IEnumerable<ProcedureDescriptor> Procedures { get; init; }
}