using System.Text.Json;
using AiPipeline.Orchestration.Contracts.Schema;
using AiPipeline.Orchestration.Runner.Application.Utils;
using AiPipeline.Orchestration.Runner.Domain.NodeTypeAggregate;

namespace AiPipeline.Orchestration.Runner.Application.Dtos.Node;

public record NodeProcedureDto(
    string Name,
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
                Name: domain.Id,
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