using AiPipeline.Orchestration.Runner.Application.Pipeline.Commands.RunPipeline;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Commands.RunPipelineAwaited;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;
using AiPipeline.Orchestration.Runner.WebApi.Contracts.Pipelines.RunAsync;
using AiPipeline.Orchestration.Runner.WebApi.Contracts.Pipelines.RunAwaitedAsync;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.WebApi.Controllers;

[ApiController]
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
        var command = new RunPipelineCommand(
            new RunPipelineDto(
                Input: request.Input,
                Steps: request.Steps
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
        var command = new RunPipelineAwaitedCommand(
            Dto: new RunPipelineDto(
                Input: request.Input,
                Steps: request.Steps
            ),
            Timeout: TimeSpan.FromSeconds(request.TimeoutSeconds)
        );
        var result = await _mediator.Send(command, cancellationToken);

        return result.ToActionResult<GetPipelineResultDto, RunPipelineAwaitedAsyncResponse>(
            onSuccess: dto => new RunPipelineAwaitedAsyncResponse(dto)
        );
    }
}