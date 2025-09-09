using AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;
using AiPipeline.TireOcr.TasyDbMatcher.Application.Queries.GetTasyDbEntriesForTireCode;
using AiPipeline.TireOcr.TasyDbMatcher.WebApi.Contracts.GetMatchesInTireDb;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.TasyDbMatcher.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
public class TireDbMatchesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TireDbMatchesController> _logger;

    public TireDbMatchesController(IMediator mediator, ILogger<TireDbMatchesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<GetMatchesInTireDbResponse>> GetMatchesInTireDb(
        [FromBody] GetMatchesInTireDbRequest request)
    {
        var query = new GetTasyDbEntriesForTireCodeQuery(request.TireCode, MaxEntries: null);
        var result = await _mediator.Send(query);

        return result.ToActionResult<List<TireDbMatchDto>, GetMatchesInTireDbResponse>(
            onSuccess: dto => new GetMatchesInTireDbResponse(dto)
        );
    }
}