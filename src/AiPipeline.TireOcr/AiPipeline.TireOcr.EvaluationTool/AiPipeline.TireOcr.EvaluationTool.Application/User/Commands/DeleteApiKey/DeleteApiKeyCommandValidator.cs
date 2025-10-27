using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Commands.DeleteApiKey;

public class DeleteApiKeyCommandValidator : AbstractValidator<DeleteApiKeyCommand>
{
    public DeleteApiKeyCommandValidator()
    {
        RuleFor(x => x.UserId.ToString())
            .IsGuid();

        RuleFor(x => x.DeletingUserId.ToString())
            .IsGuid();

        RuleFor(x => x.Name)
            .NotEmpty();
    }
}