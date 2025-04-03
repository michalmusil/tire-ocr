using Microsoft.AspNetCore.Mvc;
using TireOcr.RunnerPrototype.Contracts.RunSingle;
using TireOcr.RunnerPrototype.Extensions;

namespace TireOcr.RunnerPrototype.Controllers;

[ApiController]
[Route("[controller]")]
public class RunController : ControllerBase
{
    private readonly ILogger<RunController> _logger;

    public RunController(ILogger<RunController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<RunSingleResponse>> Single([FromForm] RunSingleRequest request)
    {
        var imageData = await request.Image.ToByteArray();

        // var result = await _mediator.Send(query);
        //
        // return result.ToActionResult<OcrResultDto, PerformOcrResponse>(
        //     onSuccess: dto => new PerformOcrResponse(dto.DetectedCode)
        // );
        return Ok(new RunSingleResponse());
    }
}