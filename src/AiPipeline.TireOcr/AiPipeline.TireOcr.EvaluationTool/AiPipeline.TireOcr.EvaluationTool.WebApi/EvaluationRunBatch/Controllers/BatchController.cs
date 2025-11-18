using AiPipeline.TireOcr.EvaluationTool.Application.Common.Services;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Commands.DeleteEvaluationBatch;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Commands.RunEvaluationBatch;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Commands.UpdateEvaluationBatch;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Queries.GetEvaluationRunBatchById;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Queries.GetEvaluationRunBatchCsvExport;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Queries.GetEvaluationRunBatchesPaginated;
using AiPipeline.TireOcr.EvaluationTool.WebApi.EvaluationRunBatch.Contracts.Batch.GetById;
using AiPipeline.TireOcr.EvaluationTool.WebApi.EvaluationRunBatch.Contracts.Batch.GetPaginated;
using AiPipeline.TireOcr.EvaluationTool.WebApi.EvaluationRunBatch.Contracts.Batch.RunBatchForm;
using AiPipeline.TireOcr.EvaluationTool.WebApi.EvaluationRunBatch.Contracts.Batch.RunBatchJsonOnly;
using AiPipeline.TireOcr.EvaluationTool.WebApi.EvaluationRunBatch.Contracts.Batch.UpdateBatch;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.EvaluationRunBatch.Controllers;

[ApiController]
[Authorize]
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
    public async Task<ActionResult<GetBatchesPaginatedResponse>> GetAllResults(
        [FromQuery] GetBatchesPaginatedRequest request)
    {
        var query = new GetEvaluationRunBatchesPaginatedQuery(
            SearchTerm: request.SearchTerm,
            Pagination: new PaginationParams(PageNumber: request.PageNumber, request.PageSize)
        );
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

    [HttpGet("{id:guid}/Export")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetExportedEvaluationBach([FromRoute] Guid id)
    {
        var query = new GetEvaluationRunBatchCsvExportQuery(id);
        var result = await _mediator.Send(query);

        return result.Map(
            onSuccess: csvData => File(csvData, "text/csv", fileDownloadName: $"{id.ToString()}.csv"),
            onFailure: failures =>
            {
                var primaryFailure = failures.FirstOrDefault();
                var otherFailures = failures.Skip(1).ToArray();

                return primaryFailure?.ToActionResult(otherFailures) ??
                       Problem($"Failed to export batch '{id}'", null, 500);
            }
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
            BatchTitle: request.RunTitle,
            BatchDescription: request.RunDescription
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
            BatchTitle: request.RunTitle,
            BatchDescription: request.RunDescription
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult<EvaluationRunBatchFullDto, RunBatchFormResponse>(
            onSuccess: dto => new RunBatchFormResponse(dto)
        );
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateBatchResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<UpdateBatchResponse>> UpdateRun(
        [FromRoute] Guid id,
        [FromBody] UpdateBatchRequest request
    )
    {
        var command = new UpdateEvaluationBatchCommand(
            BatchId: id,
            BatchTitle: request.Title,
            BatchDescription: request.Description
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult<EvaluationRunBatchFullDto, UpdateBatchResponse>(
            onSuccess: dto => new UpdateBatchResponse(dto)
        );
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DeleteRun([FromRoute] Guid id)
    {
        var command = new DeleteEvaluationBatchCommand(id);
        var result = await _mediator.Send(command);

        return result.ToActionResult(onSuccess: NoContent);
    }
}