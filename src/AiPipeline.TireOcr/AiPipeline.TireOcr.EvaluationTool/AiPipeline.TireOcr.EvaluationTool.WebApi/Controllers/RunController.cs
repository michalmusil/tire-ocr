using AiPipeline.TireOcr.EvaluationTool.Application.Commands.RunEvaluationBatch;
using AiPipeline.TireOcr.EvaluationTool.Application.Commands.RunSingleEvaluation;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;
using AiPipeline.TireOcr.EvaluationTool.Application.Services;
using AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Run.RunBatchForm;
using AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Run.RunBatchJsonOnly;
using AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Run.RunWithImage;
using AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Run.RunWithImageUrl;
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
    private readonly ICsvParserService _csvParserService;
    private readonly ILogger<RunController> _logger;

    public RunController(IMediator mediator, ILogger<RunController> logger, ICsvParserService csvParserService)
    {
        _mediator = mediator;
        _logger = logger;
        _csvParserService = csvParserService;
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
            ExpectedTireCodeLabel: request.ExpectedTireCodeLabel,
            RunConfig: runConfig,
            RunId: request.RunId,
            RunTitle: request.RunTitle
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult<EvaluationRunDto, RunWithImageResponse>(
            onSuccess: dto => new RunWithImageResponse(dto)
        );
    }

    [HttpPost("WithImageUrl")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RunWithImageResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<RunWithImageUrlResponse>> RunWithImage(
        [FromBody] RunWithImageUrlRequest request
    )
    {
        var runConfig = new RunConfigDto(
            PreprocessingType: request.PreprocessingType,
            OcrType: request.OcrType,
            PostprocessingType: request.PostprocessingType,
            DbMatchingType: request.DbMatchingType
        );

        var command = new RunSingleEvaluationCommand(
            InputImage: null,
            InputImageUrl: request.ImageUrl,
            ExpectedTireCodeLabel: request.ExpectedTireCodeLabel,
            RunConfig: runConfig,
            RunId: request.RunId,
            RunTitle: request.RunTitle
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult<EvaluationRunDto, RunWithImageUrlResponse>(
            onSuccess: dto => new RunWithImageUrlResponse(dto)
        );
    }

    [HttpPost("Batch/Json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RunBatchJsonOnlyResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<RunBatchJsonOnlyResponse>> RunBatchJsonOnly(
        [FromBody] RunBatchJsonOnlyRequest request)
    {
        var runConfig = new RunConfigDto(
            PreprocessingType: request.PreprocessingType,
            OcrType: request.OcrType,
            PostprocessingType: request.PostprocessingType,
            DbMatchingType: request.DbMatchingType
        );

        var command = new RunEvaluationBatchCommand(
            ImageUrlsWithExpectedTireCodeLabels: request.ImageUrlsWithExpectedTireCodeLabels,
            RunConfig: runConfig,
            ProcessingBatchSize: request.ProcessingBatchSize,
            BatchId: request.RunId,
            BatchTitle: request.RunTitle
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult<EvaluationRunBatchFullDto, RunBatchJsonOnlyResponse>(
            onSuccess: dto => new RunBatchJsonOnlyResponse(dto)
        );
    }

    [HttpPost("Batch/Form")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RunBatchFormResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<RunBatchFormResponse>> RunBatchJsonOnly(
        [FromForm] RunBatchFormRequest request)
    {
        var csvFile = request.ImageUrlsWithExpectedTireCodeLabelsCsv;
        if (csvFile.Length < 1)
            return BadRequest("The CSV file must contain at least one url-label pair (label is optional)");

        var isCsvFile = Path.GetExtension(csvFile.FileName)
            .Equals(".csv", StringComparison.OrdinalIgnoreCase);
        if (!isCsvFile)
            return BadRequest("Only .csv files are accepted.");

        await using var csvFileStream = csvFile.OpenReadStream();
        var parsingResult = await _csvParserService.ParseImageUrlLabelPairs(csvFileStream);
        if (parsingResult.IsFailure)
            return UnprocessableEntity(parsingResult.PrimaryFailure?.Message ?? "Failed to parse CSV file.");


        var runConfig = new RunConfigDto(
            PreprocessingType: request.PreprocessingType,
            OcrType: request.OcrType,
            PostprocessingType: request.PostprocessingType,
            DbMatchingType: request.DbMatchingType
        );

        var command = new RunEvaluationBatchCommand(
            ImageUrlsWithExpectedTireCodeLabels: parsingResult.Data!,
            RunConfig: runConfig,
            ProcessingBatchSize: request.ProcessingBatchSize,
            BatchId: request.RunId,
            BatchTitle: request.RunTitle
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult<EvaluationRunBatchFullDto, RunBatchFormResponse>(
            onSuccess: dto => new RunBatchFormResponse(dto)
        );
    }
}