using AiPipeline.Orchestration.Contracts.Schema;
using AiPipeline.Orchestration.Runner.Domain.PipelineAggregate;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos;

public record PipelineStepDto(
    string Id,
    string NodeId,
    string NodeProcedureId,
    int SchemaVersion,
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
            InputSchema: domain.InputSchema,
            OutputSchema: domain.OutputSchema
        );
    }
}