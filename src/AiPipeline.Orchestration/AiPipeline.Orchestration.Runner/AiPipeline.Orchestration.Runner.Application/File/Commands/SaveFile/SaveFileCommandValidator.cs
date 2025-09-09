using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.Orchestration.Runner.Application.File.Commands.SaveFile;

public class SaveFileCommandValidator : AbstractValidator<SaveFileCommand>
{
    public SaveFileCommandValidator()
    {
        RuleFor(x => x.UserId.ToString())
            .IsGuid();
        RuleFor(x => x.FileStream)
            .NotEmpty();
        RuleFor(x => x.ContentType)
            .NotEmpty();
    }
}