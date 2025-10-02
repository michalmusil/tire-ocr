using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Preprocessing.Application.Queries.GetPreprocessedImage;
using TireOcr.Preprocessing.Application.Queries.GetResizedImage;
using TireOcr.Preprocessing.WebApi.Contracts.ExtractRoi;
using TireOcr.Preprocessing.WebApi.Contracts.ResizeToMaxSide;
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

    [HttpPost("ExtractRoi")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ExtractRoiResponse>> PreprocessImage([FromForm] ExtractRoiRequest request)
    {
        var imageData = await request.Image.ToByteArray();
        var query = new GetPreprocessedImageQuery(imageData, request.Image.FileName, request.Image.ContentType);
        var result = await _mediator.Send(query);

        return result.ToActionResult<PreprocessedImageDto, ExtractRoiResponse>(
            onSuccess: dto =>
            {
                var base64Data = Convert.ToBase64String(dto.ImageData);
                var response = new ExtractRoiResponse(
                    FileName: dto.Name,
                    ContentType: dto.ContentType,
                    Base64ImageData: base64Data,
                    DurationMs: dto.DurationMs
                );
                return response;
            });
    }

    [HttpPost("ResizeToMaxSide")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ResizeToMaxSideResponse>> ResizeToMaxSide([FromForm] ResizeToMaxSideRequest request)
    {
        var imageData = await request.Image.ToByteArray();
        var query = new GetResizedImageQuery(
            ImageData: imageData,
            ImageName: request.Image.FileName,
            OriginalContentType: request.Image.ContentType,
            MaxImageSideDimension: request.MaxSidePixels
        );
        var result = await _mediator.Send(query);

        return result.ToActionResult<PreprocessedImageDto, ResizeToMaxSideResponse>(
            onSuccess: dto =>
            {
                var base64Data = Convert.ToBase64String(dto.ImageData);
                var response = new ResizeToMaxSideResponse(
                    FileName: dto.Name,
                    ContentType: dto.ContentType,
                    Base64ImageData: base64Data,
                    DurationMs: dto.DurationMs
                );
                return response;
            });
    }
}