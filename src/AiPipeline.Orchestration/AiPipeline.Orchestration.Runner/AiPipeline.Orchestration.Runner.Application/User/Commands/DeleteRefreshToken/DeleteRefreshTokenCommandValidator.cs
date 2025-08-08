using FluentValidation;

namespace AiPipeline.Orchestration.Runner.Application.User.Commands.DeleteRefreshToken;

public class DeleteRefreshTokenCommandValidator : AbstractValidator<DeleteRefreshTokenCommand>
{
    public DeleteRefreshTokenCommandValidator()
    {
        RuleFor(c => c.AccessToken)
            .NotNull()
            .NotEmpty();

        RuleFor(c => c.RefreshToken)
            .NotNull()
            .NotEmpty();
    }
}