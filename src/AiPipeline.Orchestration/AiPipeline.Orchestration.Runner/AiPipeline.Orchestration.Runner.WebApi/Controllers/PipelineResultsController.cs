using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Queries.GetPipelineResults;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Queries.GetResultOfPipeline;
using AiPipeline.Orchestration.Runner.WebApi.Contracts.PipelineResults.GetAllResults;
using AiPipeline.Orchestration.Runner.WebApi.Contracts.PipelineResults.GetResultOfPipeline;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
public class PipelineResultsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PipelineResultsController> _logger;

    public PipelineResultsController(IMediator mediator, ILogger<PipelineResultsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<GetAllResultsResponse>> GetAllResults([FromQuery] GetAllResultsRequest request)
    {
        var query = new GetPipelineResultsQuery(new PaginationParams(request.PageNumber, request.PageSize));
        var result = await _mediator.Send(query);

        return result.ToActionResult<PaginatedCollection<GetPipelineResultDto>, GetAllResultsResponse>(
            onSuccess: dto => new GetAllResultsResponse(dto.Items, dto.Pagination)
        );
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<GetResultOfPipelineResponse>> GetResultOfPipeline([FromRoute] Guid id)
    {
        var query = new GetResultOfPipelineQuery(id);
        var result = await _mediator.Send(query);

        return result.ToActionResult<GetPipelineResultDto, GetResultOfPipelineResponse>(
            onSuccess: dto => new GetResultOfPipelineResponse(dto)
        );
    }
}