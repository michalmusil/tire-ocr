using AiPipeline.TireOcr.EvaluationTool.Application.Commands.RunEvaluationBatch;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;
using AiPipeline.TireOcr.EvaluationTool.Application.Queries.GetEvaluationRunBatchById;
using AiPipeline.TireOcr.EvaluationTool.Application.Queries.GetEvaluationRunBatchesPaginated;
using AiPipeline.TireOcr.EvaluationTool.Application.Services;
using AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Batch.GetById;
using AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Batch.GetPaginated;
using AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Batch.RunBatchForm;
using AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Batch.RunBatchJsonOnly;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
public class BatchController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICsvParserService _csvParserService;
    private readonly ILogger<BatchController> _logger;

    public BatchController(IMediator mediator, ILogger<BatchController> logger, ICsvParserService csvParserService)
    {
        _mediator = mediator;
        _logger = logger;
        _csvParserService = csvParserService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<GetBatchesPaginatedResponse>> GetAllResults([FromQuery] GetBatchesPaginatedRequest request)
    {
        var query = new GetEvaluationRunBatchesPaginatedQuery(
            new PaginationParams(request.PageNumber, request.PageSize));
        var result = await _mediator.Send(query);

        return result.ToActionResult<PaginatedCollection<EvaluationRunBatchLightDto>, GetBatchesPaginatedResponse>(
            onSuccess: dto => new GetBatchesPaginatedResponse(dto.Items, dto.Pagination)
        );
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<GetBatchByIdResponse>> GetResultOfPipeline([FromRoute] Guid id)
    {
        var query = new GetEvaluationRunBatchByIdQuery(id);
        var result = await _mediator.Send(query);

        return result.ToActionResult<EvaluationRunBatchFullDto, GetBatchByIdResponse>(
            onSuccess: dto => new GetBatchByIdResponse(dto)
        );
    }

    [HttpPost("Json")]
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

    [HttpPost("Form")]
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