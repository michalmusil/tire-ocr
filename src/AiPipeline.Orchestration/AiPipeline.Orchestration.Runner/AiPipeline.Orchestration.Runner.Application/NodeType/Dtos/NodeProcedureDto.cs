using AiPipeline.Orchestration.Runner.Domain.NodeTypeAggregate;
using AiPipeline.Orchestration.Shared.Contracts.Schema;

namespace AiPipeline.Orchestration.Runner.Application.NodeType.Dtos;

public record NodeProcedureDto(
    string Id,
    int SchemaVersion,
    IApElement Input,
    IApElement Output
)
{
    public static NodeProcedureDto? FromDomain(NodeProcedure domain)
    {
        try
        {
            var input = domain.InputSchema;
            var output = domain.OutputSchema;

            return new NodeProcedureDto
            (
                Id: domain.Id,
                SchemaVersion: domain.SchemaVersion,
                Input: input,
                Output: output
            );
        }
        catch
        {
            return null;
        }
    }
}