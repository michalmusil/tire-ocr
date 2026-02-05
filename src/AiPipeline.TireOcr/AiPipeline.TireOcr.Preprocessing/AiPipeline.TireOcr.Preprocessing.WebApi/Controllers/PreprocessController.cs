using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TireOcr.Preprocessing.Application.Commands.ExtractImageSlices;
using TireOcr.Preprocessing.Application.Commands.ExtractTireCodeRoi;
using TireOcr.Preprocessing.Application.Commands.ResizeImage;
using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Preprocessing.WebApi.Contracts.ExtractRoi;
using TireOcr.Preprocessing.WebApi.Contracts.ExtractSlices;
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
    public async Task<ActionResult<ExtractRoiResponse>> ExtractRoi([FromForm] ExtractRoiRequest roiRequest)
    {
        var imageData = await roiRequest.Image.ToByteArray();
        var command = new ExtractTireCodeRoiCommand(
            ImageData: imageData,
            ImageName: roiRequest.Image.FileName,
            OriginalContentType: roiRequest.Image.ContentType,
            EnhanceCharacters: roiRequest.EnhanceCharacters
        );
        var result = await _mediator.Send(command);

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

    [HttpPost("ExtractRoi/File")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> ExtractRoiReturnFile([FromForm] ExtractRoiRequest roiRequest)
    {
        var imageData = await roiRequest.Image.ToByteArray();
        var command = new ExtractTireCodeRoiCommand(
            ImageData: imageData,
            ImageName: roiRequest.Image.FileName,
            OriginalContentType: roiRequest.Image.ContentType,
            EnhanceCharacters: roiRequest.EnhanceCharacters
        );
        var result = await _mediator.Send(command);

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
        var command = new ResizeImageCommand(
            ImageData: imageData,
            ImageName: request.Image.FileName,
            OriginalContentType: request.Image.ContentType,
            MaxImageSideDimension: request.MaxSidePixels
        );
        var result = await _mediator.Send(command);

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

    [HttpPost("ResizeToMaxSide/File")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> ResizeToMaxSideReturnFile([FromForm] ResizeToMaxSideRequest request)
    {
        var imageData = await request.Image.ToByteArray();
        var command = new ResizeImageCommand(
            ImageData: imageData,
            ImageName: request.Image.FileName,
            OriginalContentType: request.Image.ContentType,
            MaxImageSideDimension: request.MaxSidePixels
        );
        var result = await _mediator.Send(command);

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

    [HttpPost("ExtractSlices")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ExtractSlicesResponse>> ExtractSlices(
        [FromForm] ExtractSlicesRequest request)
    {
        var imageData = await request.Image.ToByteArray();
        var command = new ExtractImageSlicesCommand(
            ImageData: imageData,
            ImageName: request.Image.FileName,
            OriginalContentType: request.Image.ContentType,
            NumberOfSlices: request.NumberOfSlices
        );
        var result = await _mediator.Send(command);

        return result.ToActionResult<PreprocessedImageDto, ExtractSlicesResponse>(
            onSuccess: dto =>
            {
                var base64Data = Convert.ToBase64String(dto.ImageData);
                var response = new ExtractSlicesResponse(
                    FileName: dto.Name,
                    ContentType: dto.ContentType,
                    Base64ImageData: base64Data,
                    DurationMs: dto.DurationMs
                );
                return response;
            });
    }

    [HttpPost("ExtractSlices/File")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> ExtractSlicesReturnFile(
        [FromForm] ExtractSlicesRequest request)
    {
        var imageData = await request.Image.ToByteArray();
        var command = new ExtractImageSlicesCommand(
            ImageData: imageData,
            ImageName: request.Image.FileName,
            OriginalContentType: request.Image.ContentType,
            NumberOfSlices: request.NumberOfSlices
        );
        var result = await _mediator.Send(command);

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