using AiPipeline.Orchestration.Runner.Application.NodeType.Commands;
using AiPipeline.Orchestration.Runner.Application.NodeType.Dtos;
using AiPipeline.Orchestration.Shared.Contracts.Events.NodeAdvertisement;
using MediatR;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.WebApi.Consumers;

public class NodeAdvertisedHandler
{
    private readonly IMediator _mediator;
    private readonly ILogger<NodeAdvertisedHandler> _logger;

    public NodeAdvertisedHandler(IMediator mediator, ILogger<NodeAdvertisedHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }


    public async Task HandleAsync(NodeAdvertised message)
    {
        _logger.LogInformation($"Message consumed: {@message}");
        var dto = new SaveNodeDto(
            NodeId: message.NodeId,
            Procedures: message.Procedures
                .Select(p => new SaveNodeProcedureDto(
                        ProcedureId: p.Id,
                        NodeTypeId: message.NodeId,
                        SchemaVersion: p.SchemaVersion,
                        InputSchema: p.Input,
                        OutputSchema: p.Output
                    )
                )
        );
        var result = await _mediator.Send(new SaveNodeTypeCommand(dto));
        if (result.IsFailure)
        {
            _logger.LogError(
                $"Failed to save node type {message.NodeId} advertisement: {result.PrimaryFailure!.Code} - {result.PrimaryFailure.Message}"
            );
            var failure = result.PrimaryFailure ?? new Failure(500, "Failed to save node type");
            failure.ThrowAsException();
        }
    }
}