using FluentValidation;

namespace AiPipeline.Orchestration.Runner.Application.User.Commands.LogIn;

public class LogInCommandValidator : AbstractValidator<LogInCommand>
{
    public LogInCommandValidator()
    {
        RuleFor(c => c.Username)
            .NotNull()
            .NotEmpty();

        RuleFor(c => c.Password)
            .NotNull()
            .NotEmpty();
    }
}