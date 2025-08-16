using AiPipeline.Orchestration.Runner.Application.PipelineResultBatch.Dtos;
using AiPipeline.Orchestration.Runner.Application.PipelineResultBatch.Queries.GetResultBatchById;
using AiPipeline.Orchestration.Runner.WebApi.Contracts.PipelineBatches.GetBatchResultsById;
using AiPipeline.Orchestration.Runner.WebApi.Extensions;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.WebApi.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
public class PipelineBatchesControlller : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PipelineBatchesControlller> _logger;

    public PipelineBatchesControlller(IMediator mediator, ILogger<PipelineBatchesControlller> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("{batchId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<GetBatchResultsByIdResponse>> GetBatchResultsByIdAsync([FromRoute] Guid batchId)
    {
        var loggedInUser = HttpContext.GetLoggedInUserOrThrow();
        var query = new GetResultBatchByIdQuery(
            batchId,
            loggedInUser.Id
        );
        var result = await _mediator.Send(query);

        return result.ToActionResult<GetPipelineResultBatchDto, GetBatchResultsByIdResponse>(
            onSuccess: dto => new GetBatchResultsByIdResponse(dto)
        );
    }
}