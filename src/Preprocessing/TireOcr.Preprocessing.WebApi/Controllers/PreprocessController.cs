using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TireOcr.Preprocessing.WebApi.Contracts.Preprocess;
using TireOcr.Preprocessing.WebApi.Extensions;

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
    public async Task<ActionResult> PreprocessImage([FromForm] PreprocessImageRequest request)
    {
        var imageData = await request.Image.ToByteArray();
        
        return File(imageData, FallbackContentType);
    }
}