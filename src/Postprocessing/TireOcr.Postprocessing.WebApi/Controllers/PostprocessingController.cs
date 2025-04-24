using Asp.Versioning;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TireOcr.Postprocessing.Application.Dtos;
using TireOcr.Postprocessing.Application.Queries.TireCodePostprocessing;
using TireOcr.Postprocessing.WebApi.Contracts.PerformPostprocessing;
using TireOcr.Shared.Result;

namespace TireOcr.Postprocessing.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
public class PostprocessingController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PostprocessingController> _logger;

    public PostprocessingController(IMediator mediator, ILogger<PostprocessingController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PerformPostprocessingResponse>> PerformPostprocessing(
        [FromBody] PerformPostprocessingRequest request)
    {
        var query = new TireCodePostprocessingQuery(request.RawTireCode);
        var result = await _mediator.Send(query);

        return result.ToActionResult<ProcessedTireCodeResultDto, PerformPostprocessingResponse>(
            onSuccess: dto => dto.Adapt<PerformPostprocessingResponse>()
        );
    }
}