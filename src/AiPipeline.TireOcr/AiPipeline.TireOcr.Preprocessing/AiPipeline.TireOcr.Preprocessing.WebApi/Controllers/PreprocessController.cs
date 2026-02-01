using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Preprocessing.Application.Queries.GetImageSlices;
using TireOcr.Preprocessing.Application.Queries.GetResizedImage;
using TireOcr.Preprocessing.Application.Queries.GetTireCodeRoi;
using TireOcr.Preprocessing.WebApi.Contracts.Extract;
using TireOcr.Preprocessing.WebApi.Contracts.ExtractSlicesComposition;
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

    public PreprocessController(IMediator mediator, ILogger<PreprocessController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("ExtractRoi")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ExtractResponse>> ExtractRoi([FromForm] ExtractRequest request)
    {
        var imageData = await request.Image.ToByteArray();
        var query = new GetTireCodeRoiQuery(
            ImageData: imageData,
            ImageName: request.Image.FileName,
            OriginalContentType: request.Image.ContentType,
            EnhanceCharacters: request.EnhanceCharacters
        );
        var result = await _mediator.Send(query);

        return result.ToActionResult<PreprocessedImageDto, ExtractResponse>(
            onSuccess: dto =>
            {
                var base64Data = Convert.ToBase64String(dto.ImageData);
                var response = new ExtractResponse(
                    FileName: dto.Name,
                    ContentType: dto.ContentType,
                    Base64ImageData: base64Data,
                    DurationMs: dto.DurationMs
                );
                return response;
            });
    }

    [HttpPost("ExtractRoiReturnFile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> ExtractRoiReturnFile([FromForm] ExtractRequest request)
    {
        var imageData = await request.Image.ToByteArray();
        var query = new GetTireCodeRoiQuery(
            ImageData: imageData,
            ImageName: request.Image.FileName,
            OriginalContentType: request.Image.ContentType,
            EnhanceCharacters: request.EnhanceCharacters
        );
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

    [HttpPost("ExtractSlicesComposition")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ExtractSlicesCompositionResponse>> ExtractSlicesComposition(
        [FromForm] ExtractSlicesCompositionRequest compositionRequest)
    {
        var imageData = await compositionRequest.Image.ToByteArray();
        var query = new GetImageSlicesQuery(
            ImageData: imageData,
            ImageName: compositionRequest.Image.FileName,
            OriginalContentType: compositionRequest.Image.ContentType,
            NumberOfSlices: compositionRequest.NumberOfSlices
        );
        var result = await _mediator.Send(query);

        return result.ToActionResult<PreprocessedImageDto, ExtractSlicesCompositionResponse>(
            onSuccess: dto =>
            {
                var base64Data = Convert.ToBase64String(dto.ImageData);
                var response = new ExtractSlicesCompositionResponse(
                    FileName: dto.Name,
                    ContentType: dto.ContentType,
                    Base64ImageData: base64Data,
                    DurationMs: dto.DurationMs
                );
                return response;
            });
    }

    [HttpPost("ExtractSlicesCompositionReturnFile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> ExtractSlicesCompositionReturnFile(
        [FromForm] ExtractSlicesCompositionRequest compositionRequest)
    {
        var imageData = await compositionRequest.Image.ToByteArray();
        var query = new GetImageSlicesQuery(
            ImageData: imageData,
            ImageName: compositionRequest.Image.FileName,
            OriginalContentType: compositionRequest.Image.ContentType,
            NumberOfSlices: compositionRequest.NumberOfSlices
        );
        var result = await _mediator.Send(query);

        return result.Map(
            onSuccess: dto => File(dto.ImageData, dto.ContentType),
            onFailure: failures =>
            {
                var primaryFailure = failures.FirstOrDefault();
                var otherFailures = failures.Skip(1).ToArray();

                return primaryFailure?.ToActionResult(otherFailures) ??
                       Problem("Failed to extract image slices", null, 500);
            }
        );
    }
}