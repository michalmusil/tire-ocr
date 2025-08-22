using AiPipeline.Orchestration.Runner.Domain.PipelineAggregate;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos;

public record PipelineStepDto(
    string Id,
    string NodeId,
    string NodeProcedureId,
    int SchemaVersion,
    int Order,
    string? OutputValueSelector,
    IApElement InputSchema,
    IApElement OutputSchema
)
{
    public static PipelineStepDto FromDomain(PipelineStep domain)
    {
        return new PipelineStepDto(
            Id: domain.Id.ToString(),
            NodeId: domain.NodeId,
            NodeProcedureId: domain.NodeProcedureId,
            SchemaVersion: domain.SchemaVersion,
            Order: domain.Order,
            OutputValueSelector: domain.OutputValueSelector,
            InputSchema: domain.InputSchema,
            OutputSchema: domain.OutputSchema
        );
    }
}