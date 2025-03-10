using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TireOcr.Preprocessing.Application.Queries.GetPreprocessedImage;
using TireOcr.Preprocessing.WebApi.Contracts.Preprocess;
using TireOcr.Preprocessing.WebApi.Extensions;
using TireOcr.Shared.Result;

namespace TireOcr.Preprocessing.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
public class PreprocessController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PreprocessController> _logger;

    private const string FallbackContentType = "application/octet-stream";

    public PreprocessController(IMediator mediator, ILogger<PreprocessController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> PreprocessImage([FromForm] PreprocessImageRequest request)
    {
        var imageData = await request.Image.ToByteArray();
        var query = new GetPreprocessedImageQuery(imageData, request.Image.FileName, request.Image.ContentType);
        var result = await _mediator.Send(query);

        return result.Map(
            onSuccess: dto => File(dto.ImageData, dto.ContentType),
            onFailure: failures =>
            {
                var primaryFailure = failures.FirstOrDefault();
                var otherFailures = failures.Skip(1).ToArray();

                return primaryFailure?.ToActionResult(otherFailures) ??
                       Problem("Failed to preprocess image", null, 500);
            }
        );
    }
}