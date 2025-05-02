using Microsoft.AspNetCore.Mvc;
using TireOcr.RunnerPrototype.Contracts.RunBatch;
using TireOcr.RunnerPrototype.Contracts.RunSingle;
using TireOcr.RunnerPrototype.Dtos;
using TireOcr.RunnerPrototype.Dtos.Batch;
using TireOcr.RunnerPrototype.Extensions;
using TireOcr.RunnerPrototype.Models;
using TireOcr.RunnerPrototype.Services.TireOcr;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Controllers;

[ApiController]
[Route("[controller]")]
public class RunController : ControllerBase
{
    private readonly ITireOcrService _tireOcrService;
    private readonly ILogger<RunController> _logger;

    public RunController(ITireOcrService tireOcrService, ILogger<RunController> logger)
    {
        _tireOcrService = tireOcrService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<RunSingleResponse>> Single([FromForm] RunSingleRequest request)
    {
        var imageData = await request.Image.ToByteArray();
        var imageToProcess = new Image(imageData, request.Image.FileName, request.Image.ContentType);

        var result = await _tireOcrService.RunSingleOcrPipelineAsync(imageToProcess, request.DetectorType);

        return result.ToActionResult<TireOcrResult, RunSingleResponse>(
            onSuccess: res => new RunSingleResponse(res)
        );
    }

    [HttpPost("batch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<RunBatchResponse>> Batch([FromForm] RunBatchRequest request)
    {
        var result = await _tireOcrService.RunOcrPipelineBatchAsync(
            request.ImageUrls,
            request.BatchSize,
            request.DetectorType
        );

        return result.ToActionResult<TireOcrBatchResult, RunBatchResponse>(
            onSuccess: res => new RunBatchResponse(res)
        );
    }
}