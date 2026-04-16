using AiPipeline.TireOcr.EvaluationTool.Application.User.Commands.DeleteUser;
using AiPipeline.TireOcr.EvaluationTool.Application.User.Commands.UpdateUser;
using AiPipeline.TireOcr.EvaluationTool.Application.User.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.User.Queries.GetUserById;
using AiPipeline.TireOcr.EvaluationTool.WebApi.Common.Extensions;
using AiPipeline.TireOcr.EvaluationTool.WebApi.User.Contracts.User.GetById;
using AiPipeline.TireOcr.EvaluationTool.WebApi.User.Contracts.User.Update;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.User.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<GetUserByIdResponse>> GetById([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id));
        return result.ToActionResult<UserDto, GetUserByIdResponse>(
            onSuccess: userDto => new GetUserByIdResponse(userDto)
        );
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<UpdateUserResponse>> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateUserRequest contract
    )
    {
        var updatingUser = HttpContext.GetLoggedInUser();
        if (updatingUser is null || updatingUser.IsAuthenticatedViaApiKey)
            return Unauthorized("User must be logged in to update their account");
        var command = new UpdateUserCommand(
            UpdatingUserId: updatingUser.Id,
            UserToUpdateId: id,
            Username: contract.Username,
            Password: contract.Password
        );

        var result = await _mediator.Send(command);
        return result.ToActionResult<UserDto, UpdateUserResponse>(
            onSuccess: userDto => new UpdateUserResponse(userDto)
        );
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deletingUser = HttpContext.GetLoggedInUser();
        if (deletingUser is null || deletingUser.IsAuthenticatedViaApiKey)
            return Unauthorized("User must be logged in to delete their account");
        var command = new DeleteUserCommand(
            DeletingUserId: deletingUser.Id,
            UserToDeleteId: id
        );

        var result = await _mediator.Send(command);
        return result.ToActionResult(
            onSuccess: NoContent
        );
    }
}