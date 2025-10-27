using AiPipeline.TireOcr.EvaluationTool.Application.User.Commands.AddApiKey;
using AiPipeline.TireOcr.EvaluationTool.Application.User.Commands.DeleteApiKey;
using AiPipeline.TireOcr.EvaluationTool.Application.User.Commands.LogIn;
using AiPipeline.TireOcr.EvaluationTool.Application.User.Commands.RefreshToken;
using AiPipeline.TireOcr.EvaluationTool.Application.User.Commands.RegisterUser;
using AiPipeline.TireOcr.EvaluationTool.Application.User.Dtos;
using AiPipeline.TireOcr.EvaluationTool.WebApi.Common.Extensions;
using AiPipeline.TireOcr.EvaluationTool.WebApi.User.Contracts.Auth.CreateApiKey;
using AiPipeline.TireOcr.EvaluationTool.WebApi.User.Contracts.Auth.LogIn;
using AiPipeline.TireOcr.EvaluationTool.WebApi.User.Contracts.Auth.RefreshToken;
using AiPipeline.TireOcr.EvaluationTool.WebApi.User.Contracts.Auth.Register;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.User.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("Login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<LogInResponse>> LogIn([FromBody] LogInRequest contract)
    {
        var command = new LogInCommand(
            contract.Username,
            contract.Password
        );
        var result = await _mediator.Send(command);

        return result.ToActionResult<AuthenticatedUserDto, LogInResponse>(
            onSuccess: userDto => new LogInResponse(userDto)
        );
    }

    [HttpPost("Register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<RegisterUserResponse>> Register(
        [FromBody] RegisterUserRequest contract)
    {
        var command = new RegisterUserCommand(
            contract.Id,
            contract.Username,
            contract.Password
        );
        var result = await _mediator.Send(command);

        return result.ToActionResult<UserDto, RegisterUserResponse>(
            onSuccess: userDto =>
            {
                var createdAt = Url.Action(
                    nameof(UsersController.GetById),
                    nameof(UsersController),
                    new { id = userDto.Id }
                );
                return Created(createdAt, userDto);
            });
    }

    [HttpPost("Refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<RefreshTokenResponse>> RefreshToken(
        [FromBody] RefreshTokenRequest contract)
    {
        var command = new RefreshTokenCommand(
            AccessToken: contract.AccessToken,
            RefreshToken: contract.RefreshToken
        );
        var result = await _mediator.Send(command);

        return result.ToActionResult<AuthenticatedUserDto, RefreshTokenResponse>(
            onSuccess: userDto => new RefreshTokenResponse(userDto)
        );
        ;
    }

    [Authorize]
    [HttpPost("ApiKey")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<CreateApiKeyResponse>> CreateApiKey([FromBody] CreateApiKeyRequest contract)
    {
        var requestingUser = HttpContext.GetLoggedInUser();
        if (requestingUser is null || requestingUser.IsAuthenticatedViaApiKey)
            return Unauthorized("Only logged in users can create api keys");

        var command = new AddApiKeyCommand(
            UserId: requestingUser.Id,
            CreatorUserId: requestingUser.Id,
            Name: contract.Name,
            ValidUntil: contract.ValidUntil
        );
        var result = await _mediator.Send(command);

        return result.ToActionResult<NewApiKeyDto, CreateApiKeyResponse>(
            onSuccess: dto => new CreateApiKeyResponse(dto)
        );
    }

    [Authorize]
    [HttpDelete("ApiKey/{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DeleteApiKey([FromRoute] string name)
    {
        var requestingUser = HttpContext.GetLoggedInUser();
        if (requestingUser?.IsAuthenticatedViaApiKey ?? true)
            return Unauthorized("Only logged in users can delete their api keys");

        var command = new DeleteApiKeyCommand(
            UserId: requestingUser.Id,
            DeletingUserId: requestingUser.Id,
            Name: name
        );
        var result = await _mediator.Send(command);

        return result.ToActionResult(
            onSuccess: NoContent
        );
    }
}