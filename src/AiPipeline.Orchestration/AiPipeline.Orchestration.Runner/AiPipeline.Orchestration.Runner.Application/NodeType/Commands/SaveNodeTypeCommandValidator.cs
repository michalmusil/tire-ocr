using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.Orchestration.Runner.Application.NodeType.Commands;

public class SaveNodeTypeCommandValidator : AbstractValidator<SaveNodeTypeCommand>
{
    public SaveNodeTypeCommandValidator()
    {
        RuleFor(x => x.Dto.NodeId.ToString())
            .IsGuid();
    }
}