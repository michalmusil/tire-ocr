using FluentValidation;

namespace AiPipeline.Orchestration.Runner.Application.File.Commands.SaveFile;

public class SaveFileCommandValidator : AbstractValidator<SaveFileCommand>
{
    public SaveFileCommandValidator()
    {
        RuleFor(x => x.FileStream)
            .NotEmpty();
        RuleFor(x => x.ContentType)
            .NotEmpty();
    }
}