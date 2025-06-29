using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.Orchestration.Runner.Application.File.Commands.RemoveFile;

public class RemoveFileCommandValidator : AbstractValidator<RemoveFileCommand>
{
    public RemoveFileCommandValidator()
    {
        RuleFor(x => x.Id.ToString())
            .IsGuid();
    }
}