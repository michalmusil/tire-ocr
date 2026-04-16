using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Commands.DeleteUser;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(c => c.DeletingUserId.ToString())
            .IsGuid();
        RuleFor(c => c.UserToDeleteId.ToString())
            .IsGuid();
    }
}