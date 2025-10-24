using System.IO.Compression;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Preprocessing.Application.Queries.GetImageSlices;
using TireOcr.Preprocessing.Application.Queries.GetResizedImage;
using TireOcr.Preprocessing.Application.Queries.GetTireCodeRoi;
using TireOcr.Preprocessing.WebApi.Contracts.Extract;
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
    public async Task<ActionResult<ExtractResponse>> ExtractRoi([FromForm] ExtractRequest request)
    {
        var imageData = await request.Image.ToByteArray();
        var query = new GetTireCodeRoiQuery(
            ImageData: imageData,
            ImageName: request.Image.FileName,
            OriginalContentType: request.Image.ContentType,
            RemoveBackground: request.RemoveBackground
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
            RemoveBackground: request.RemoveBackground
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

    [HttpPost("ExtractSlices")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ExtractSlicesResponse>> ExtractSlices([FromForm] ExtractSlicesRequest request)
    {
        var imageData = await request.Image.ToByteArray();
        var query = new GetImageSlicesQuery(
            ImageData: imageData,
            ImageName: request.Image.FileName,
            OriginalContentType: request.Image.ContentType,
            NumberOfSlices: request.NumberOfSlices
        );
        var result = await _mediator.Send(query);

        return result.ToActionResult<ImageSlicesResultDto, ExtractSlicesResponse>(
            onSuccess: dto =>
            {
                var slices = dto.Slices.Select(s =>
                    new SliceDto(
                        FileName: s.Name,
                        ContentType: s.ContentType,
                        Base64ImageData: Convert.ToBase64String(s.ImageData)
                    )
                );
                var response = new ExtractSlicesResponse(
                    Slices: slices,
                    DurationMs: dto.DurationMs
                );
                return response;
            });
    }

    [HttpPost("ExtractSlicesReturnFile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> ExtractSlicesReturnFile([FromForm] ExtractSlicesRequest request)
    {
        var imageData = await request.Image.ToByteArray();
        var query = new GetImageSlicesQuery(
            ImageData: imageData,
            ImageName: request.Image.FileName,
            OriginalContentType: request.Image.ContentType,
            NumberOfSlices: request.NumberOfSlices
        );
        var result = await _mediator.Send(query);

        return result.Map(
            onSuccess: dto =>
            {
                using var archiveStream = new MemoryStream();
                using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true))
                {
                    foreach (var slice in dto.Slices)
                    {
                        var zipArchiveEntry = archive.CreateEntry(slice.Name, CompressionLevel.Fastest);
                        using var entryStream = zipArchiveEntry.Open();
                        entryStream.Write(slice.ImageData, 0, slice.ImageData.Length);
                    }
                }

                var data = archiveStream.ToArray();
                return File(data, "application/zip", "slices.zip");
            },
            onFailure:
            failures =>
            {
                var primaryFailure = failures.FirstOrDefault();
                var otherFailures = failures.Skip(1).ToArray();

                return primaryFailure?.ToActionResult(otherFailures) ??
                       Problem("Failed to extract image slices", null, 500);
            }
        );
    }
}