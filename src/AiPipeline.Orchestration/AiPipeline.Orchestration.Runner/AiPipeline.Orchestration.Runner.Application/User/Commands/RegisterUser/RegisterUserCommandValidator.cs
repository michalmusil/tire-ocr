using FluentValidation;

namespace AiPipeline.Orchestration.Runner.Application.User.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(c => c.Username)
            .MinimumLength(2)
            .MaximumLength(30);
        RuleFor(c => c.Password)
            .MinimumLength(5)
            .MaximumLength(500);
    }
}