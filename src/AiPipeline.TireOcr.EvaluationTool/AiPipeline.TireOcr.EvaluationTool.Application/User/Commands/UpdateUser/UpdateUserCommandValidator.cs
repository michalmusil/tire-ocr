using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(c => c.UpdatingUserId.ToString())
            .IsGuid();
        RuleFor(c => c.UserToUpdateId.ToString())
            .IsGuid();
        RuleFor(c => c.Username)
            .Must(
                un => un is null || un.Length is >= 2 and <= 30
            )
            .WithMessage("Specified username has invalid length.");
        RuleFor(c => c.Password)
            .Must(
                pw => pw is null || pw.Length is >= 5 and <= 500
            )
            .WithMessage("Specified password has invalid length.");
    }
}