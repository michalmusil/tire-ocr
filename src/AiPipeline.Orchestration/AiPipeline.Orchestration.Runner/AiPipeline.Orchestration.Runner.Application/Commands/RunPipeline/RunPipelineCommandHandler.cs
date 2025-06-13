using AiPipeline.Orchestration.Runner.Application.Dtos.Pipeline;
using AiPipeline.Orchestration.Runner.Application.Repositories;
using AiPipeline.Orchestration.Runner.Domain.PipelineAggregate;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.Commands.RunPipeline;

public class RunPipelineCommandHandler : ICommandHandler<RunPipelineCommand, PipelineDto>
{
    private readonly INodeTypeRepository _nodeTypeRepository;
    private readonly ILogger<RunPipelineCommandHandler> _logger;

    public RunPipelineCommandHandler(INodeTypeRepository nodeTypeRepository,
        ILogger<RunPipelineCommandHandler> logger)
    {
        _nodeTypeRepository = nodeTypeRepository;
        _logger = logger;
    }

    public async Task<DataResult<PipelineDto>> Handle(
        RunPipelineCommand request,
        CancellationToken cancellationToken
    )
    {
        var nodeIds = request.Dto.Steps.Select(s => s.NodeId).ToArray();
        var nodes = (await _nodeTypeRepository.GetNodeTypesByIdsAsync(nodeIds))
            .ToList();

        List<PipelineStep> steps = [];
        foreach (var requestedStep in request.Dto.Steps)
        {
            var node = nodes.FirstOrDefault(n => n.Id == requestedStep.NodeId);
            if (node is null)
                return DataResult<PipelineDto>.NotFound($"Node type {requestedStep.NodeId} not found");

            var procedure = node.AvailableProcedures.FirstOrDefault(p => p.Id == requestedStep.ProcedureId);
            if (procedure is null)
                return DataResult<PipelineDto>.NotFound(
                    $"Procedure {requestedStep.ProcedureId} not found on node {node!.Id}");

            steps.Add(new PipelineStep(
                nodeId: node!.Id,
                nodeProcedureId: procedure.Id,
                schemaVersion: procedure.SchemaVersion,
                inputSchema: procedure.InputSchema,
                outputSchema: procedure.OutputSchema
            ));
        }

        var pipeline = new Pipeline(steps: steps);
        var validationResult = pipeline.Validate();
        return validationResult.Map(
            onSuccess: () => DataResult<PipelineDto>.Success(PipelineDto.FromDomain(pipeline)),
            onFailure: DataResult<PipelineDto>.Failure
        );
    }
}