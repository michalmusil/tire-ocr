using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Queries.PerformTireImageOcr;
using TireOcr.Ocr.WebApi.Contracts.PerformOcr;
using TireOcr.Ocr.WebApi.Extensions;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
public class OcrController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OcrController> _logger;

    public OcrController(IMediator mediator, ILogger<OcrController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PerformOcrResponse>> PerformOcr([FromForm] PerformOcrRequest request)
    {
        var imageData = await request.Image.ToByteArray();
        var query = new PerformTireImageOcrQuery(
            request.DetectorType,
            imageData,
            request.Image.FileName,
            request.Image.ContentType,
            true
        );

        var result = await _mediator.Send(query);

        return result.ToActionResult<OcrWithBillingDto, PerformOcrResponse>(
            onSuccess: dto => new PerformOcrResponse(dto.DetectedCode, dto.DetectedManufacturer?.ToUpper(), dto.EstimatedCosts)
        );
    }
}