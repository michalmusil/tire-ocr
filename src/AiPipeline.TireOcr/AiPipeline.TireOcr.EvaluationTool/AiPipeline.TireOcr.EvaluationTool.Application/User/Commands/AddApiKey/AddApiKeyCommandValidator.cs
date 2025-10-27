using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Commands.AddApiKey;

public class AddApiKeyCommandValidator : AbstractValidator<AddApiKeyCommand>
{
    public AddApiKeyCommandValidator()
    {
        RuleFor(x => x.UserId.ToString())
            .IsGuid();

        RuleFor(x => x.CreatorUserId.ToString())
            .IsGuid();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100);
    }
}