namespace AiPipeline.Orchestration.Shared.Contracts.Events.NodeAdvertisement;

public record NodeAdvertised
{
    public required string NodeName { get; init; }
    public required IEnumerable<ProcedureDescriptor> Procedures { get; init; }
}