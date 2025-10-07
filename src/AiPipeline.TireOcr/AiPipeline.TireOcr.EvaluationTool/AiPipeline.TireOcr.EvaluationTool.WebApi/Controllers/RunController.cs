using AiPipeline.TireOcr.EvaluationTool.Application.Commands.RunSingleEvaluation;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;
using AiPipeline.TireOcr.EvaluationTool.Application.Queries.GetEvaluationRunById;
using AiPipeline.TireOcr.EvaluationTool.Application.Queries.GetEvaluationRunsPaginated;
using AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Run.GetById;
using AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Run.GetPaginated;
using AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Run.RunWithImage;
using AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Run.RunWithImageUrl;
using AiPipeline.TireOcr.EvaluationTool.WebApi.Extensions;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TireOcr.Shared.Pagination;
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

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<GetRunsPaginatedResponse>> GetAllResults([FromQuery] GetRunsPaginatedRequest request)
    {
        var query = new GetEvaluationRunsPaginatedQuery(new PaginationParams(request.PageNumber, request.PageSize));
        var result = await _mediator.Send(query);

        return result.ToActionResult<PaginatedCollection<EvaluationRunDto>, GetRunsPaginatedResponse>(
            onSuccess: dto => new GetRunsPaginatedResponse(dto.Items, dto.Pagination)
        );
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<GetRunByIdResponse>> GetResultOfPipeline([FromRoute] Guid id)
    {
        var query = new GetEvaluationRunByIdQuery(id);
        var result = await _mediator.Send(query);

        return result.ToActionResult<EvaluationRunDto, GetRunByIdResponse>(
            onSuccess: dto => new GetRunByIdResponse(dto)
        );
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
}