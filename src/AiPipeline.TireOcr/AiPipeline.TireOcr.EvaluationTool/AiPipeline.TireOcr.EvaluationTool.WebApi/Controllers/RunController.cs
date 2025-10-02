using AiPipeline.TireOcr.EvaluationTool.Application.Commands.RunSingleEvaluation;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;
using AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Run.RunWithImage;
using AiPipeline.TireOcr.EvaluationTool.WebApi.Extensions;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
public class RunController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<RunController> _logger;

    public RunController(IMediator mediator, ILogger<RunController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    [HttpPost("WithImage")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RunWithImageResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<RunWithImageResponse>> RunWithImage(
        [FromForm] RunWithImageRequest request
    )
    {
        var imageData = await request.Image.ToByteArray();
        var imageDto = new ImageDto(
            FileName: request.Image.FileName,
            ImageData: imageData,
            ContentType: request.Image.ContentType
        );
        var runConfig = new RunConfigDto(
            PreprocessingType: request.PreprocessingType,
            OcrType: request.OcrType,
            PostprocessingType: request.PostprocessingType,
            DbMatchingType: request.DbMatchingType
        );

        var command = new RunSingleEvaluationCommand(
            InputImage: imageDto,
            InputImageUrl: null,
            ExpectedTireCode: null, // TODO: Add after parsing is implemented
            RunConfig: runConfig,
            RunId: request.RunId,
            RunTitle: request.RunTitle
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult<EvaluationRunDto, RunWithImageResponse>(
            onSuccess: dto => new RunWithImageResponse(dto)
        );
    }
    
    // [HttpPost("batch")]
    // [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RunBatchResponse))]
    // [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    // public async Task<ActionResult<RunBatchResponse>> Batch([FromForm] RunBatchRequest request)
    // {
    //     var result = await _pipelineRunnerService.RunOcrPipelineBatchAsync(
    //         request.ImageUrls,
    //         request.BatchSize,
    //         request.DetectorType
    //     );
    //
    //     return result.ToActionResult<TireOcrBatchResultDto, RunBatchResponse>(
    //         onSuccess: res => new RunBatchResponse(res)
    //     );
    // }
}