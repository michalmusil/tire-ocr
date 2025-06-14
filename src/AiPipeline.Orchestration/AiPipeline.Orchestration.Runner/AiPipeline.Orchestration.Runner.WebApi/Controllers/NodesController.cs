using AiPipeline.Orchestration.Runner.Application.NodeType.Dtos;
using AiPipeline.Orchestration.Runner.Application.NodeType.Queries.GetAvailableNodes;
using AiPipeline.Orchestration.Runner.WebApi.Contracts.Nodes.GetAllNodes;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
public class NodesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<NodesController> _logger;

    public NodesController(IMediator mediator, ILogger<NodesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<GetAllNodesResponse>> GetAllNodes([FromQuery] GetAllNodesRequest request)
    {
        var query = new GetAvailableNodesQuery(new PaginationParams(request.PageNumber, request.PageSize));
        var result = await _mediator.Send(query);

        return result.ToActionResult<PaginatedCollection<NodeDto>, GetAllNodesResponse>(
            onSuccess: dto => new GetAllNodesResponse(dto.Items, dto.Pagination)
        );
    }
}