using AiPipeline.Orchestration.Runner.Application.Pipeline.Commands.RunPipeline;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Commands.RunPipelineAwaited;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Commands.RunPipelineBatch;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;
using AiPipeline.Orchestration.Runner.WebApi.Contracts.Pipelines.Run;
using AiPipeline.Orchestration.Runner.WebApi.Contracts.Pipelines.RunAwaited;
using AiPipeline.Orchestration.Runner.WebApi.Contracts.Pipelines.RunBatch;
using AiPipeline.Orchestration.Runner.WebApi.Extensions;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.WebApi.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
public class PipelinesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PipelinesController> _logger;

    public PipelinesController(IMediator mediator, ILogger<PipelinesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<RunPipelineAsyncResponse>> RunAsync([FromBody] RunPipelineAsyncRequest request)
    {
        var loggedInUser = HttpContext.GetLoggedInUserOrThrow();
        var command = new RunPipelineCommand(
            new RunPipelineDto(
                Input: request.Input,
                Steps: request.Steps,
                UserId: loggedInUser.Id
            )
        );
        var result = await _mediator.Send(command);

        return result.ToActionResult<PipelineDto, RunPipelineAsyncResponse>(
            onSuccess: dto => new RunPipelineAsyncResponse(dto)
        );
    }

    [HttpPost("Awaited")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
    public async Task<ActionResult<RunPipelineAwaitedAsyncResponse>> RunAwaitedAsync(
        [FromBody] RunPipelineAwaitedAsyncRequest request, CancellationToken cancellationToken)
    {
        var loggedInUser = HttpContext.GetLoggedInUserOrThrow();
        var command = new RunPipelineAwaitedCommand(
            Dto: new RunPipelineDto(
                Input: request.Input,
                Steps: request.Steps,
                UserId: loggedInUser.Id
            ),
            Timeout: TimeSpan.FromSeconds(request.TimeoutSeconds)
        );
        var result = await _mediator.Send(command, cancellationToken);

        return result.ToActionResult<GetPipelineResultDto, RunPipelineAwaitedAsyncResponse>(
            onSuccess: dto => new RunPipelineAwaitedAsyncResponse(dto)
        );
    }

    [HttpPost("Batch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<RunBatchAsyncResponse>> RunBatchAsync(
        [FromBody] RunBatchAsyncRequest request,
        CancellationToken cancellationToken)
    {
        var loggedInUser = HttpContext.GetLoggedInUserOrThrow();
        var command = new RunPipelineBatchCommand(
            RunPipelineBatchDto: new RunPipelineBatchDto(
                UserId: loggedInUser.Id,
                Inputs: request.Inputs,
                Steps: request.Steps
            )
        );
        var result = await _mediator.Send(command, cancellationToken);

        return result.ToActionResult<GetPipelineBatchDto, RunBatchAsyncResponse>(
            onSuccess: dto => new RunBatchAsyncResponse(dto)
        );
    }
}