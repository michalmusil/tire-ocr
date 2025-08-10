using AiPipeline.Orchestration.Runner.Application.User.Commands.LogIn;
using AiPipeline.Orchestration.Runner.Application.User.Commands.RefreshToken;
using AiPipeline.Orchestration.Runner.Application.User.Commands.RegisterUser;
using AiPipeline.Orchestration.Runner.Application.User.Dtos;
using AiPipeline.Orchestration.Runner.WebApi.Contracts.Auth.LogIn;
using AiPipeline.Orchestration.Runner.WebApi.Contracts.Auth.RefreshToken;
using AiPipeline.Orchestration.Runner.WebApi.Contracts.Auth.Register;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.WebApi.Controllers;

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
        var hasAccessToken = HttpContext.Request.Headers.ContainsKey("Authorization");
        if (!hasAccessToken)
            return UnprocessableEntity("Missing authorization header");

        var accessToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var command = new RefreshTokenCommand(
            AccessToken: accessToken,
            RefreshToken: contract.RefreshToken
        );
        var result = await _mediator.Send(command);

        return result.ToActionResult<AuthenticatedUserDto, RefreshTokenResponse>(
            onSuccess: userDto => new RefreshTokenResponse(userDto)
        );
        ;
    }
}