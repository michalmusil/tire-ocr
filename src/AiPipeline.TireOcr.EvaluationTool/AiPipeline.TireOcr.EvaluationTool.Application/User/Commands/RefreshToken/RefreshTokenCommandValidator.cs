using FluentValidation;

namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Commands.RefreshToken;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(c => c.AccessToken)
            .NotNull()
            .NotEmpty();

        RuleFor(c => c.RefreshToken)
            .NotNull()
            .NotEmpty();
    }
}