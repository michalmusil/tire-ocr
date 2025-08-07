using AiPipeline.Orchestration.Runner.Application.File.Commands.RemoveFile;
using AiPipeline.Orchestration.Runner.Application.File.Commands.SaveFile;
using AiPipeline.Orchestration.Runner.Application.File.Dtos;
using AiPipeline.Orchestration.Runner.Application.File.Queries.GetFileById;
using AiPipeline.Orchestration.Runner.Application.File.Queries.GetFilesPaginated;
using AiPipeline.Orchestration.Runner.Application.File.Queries.GetFileWithDataById;
using AiPipeline.Orchestration.Runner.Domain.FileAggregate;
using AiPipeline.Orchestration.Runner.WebApi.Contracts.Files.GetAllFiles;
using AiPipeline.Orchestration.Runner.WebApi.Contracts.Files.GetFileById;
using AiPipeline.Orchestration.Runner.WebApi.Contracts.Files.UploadFile;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FilesController> _logger;

    public FilesController(IMediator mediator, ILogger<FilesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<GetAllFilesResponse>> GetAllFiles([FromQuery] GetAllFilesRequest request)
    {
        var pagination = new PaginationParams(request.PageNumber, request.PageSize);
        var query = new GetFilesPaginatedQuery(
            Pagination: pagination,
            ScopeFilter: request.StorageScopeFilter
        );
        var result = await _mediator.Send(query);

        return result.ToActionResult<PaginatedCollection<FileDto>, GetAllFilesResponse>(
            onSuccess: dto => new GetAllFilesResponse(dto.Items, dto.Pagination)
        );
    }

    [HttpGet("{fileId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<GetFileByIdResponse>> GetFileById([FromRoute] Guid fileId)
    {
        var query = new GetFileByIdQuery(fileId);
        var result = await _mediator.Send(query);

        return result.ToActionResult<FileDto, GetFileByIdResponse>(
            onSuccess: dto => new GetFileByIdResponse(dto)
        );
    }

    [HttpPost("Upload")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<UploadFileResponse>> UploadFile([FromForm] UploadFileRequest request)
    {
        var originalFileName = request.File.FileName;
        var contentType = request.File.ContentType;
        var fileStream = request.File.OpenReadStream();

        var command = new SaveFileCommand(
            FileStream: fileStream,
            OriginalFileName: originalFileName,
            FileStorageScope: FileStorageScope.ShortTerm,
            ContentType: contentType,
            Id: request.Id
        );

        var result = await _mediator.Send(command);
        return result.ToActionResult<FileDto, UploadFileResponse>(
            onSuccess: dto =>
            {
                var createdAt = Url.Action("GetFileById", new { fileId = dto.Id });
                var resData = new UploadFileResponse(dto);
                return Created(createdAt, resData);
            });
    }

    [HttpGet("{fileId:guid}/Download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DownloadFile([FromRoute] Guid fileId)
    {
        var command = new GetFileWithDataByIdQuery(fileId);
        var result = await _mediator.Send(command);

        return result.Map<ActionResult>(
            onSuccess: dto => File(dto.DataStream, dto.File.ContentType),
            onFailure: failures =>
            {
                var primaryFailure = failures.FirstOrDefault() ??
                                     new Failure(500, "Unknown failure while downloading a file");
                return primaryFailure.ToActionResult();
            }
        );
    }

    [HttpDelete("{fileId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DeleteFile([FromRoute] Guid fileId)
    {
        var command = new RemoveFileCommand(fileId);
        var result = await _mediator.Send(command);

        return result.ToActionResult(
            onSuccess: NoContent
        );
    }
}