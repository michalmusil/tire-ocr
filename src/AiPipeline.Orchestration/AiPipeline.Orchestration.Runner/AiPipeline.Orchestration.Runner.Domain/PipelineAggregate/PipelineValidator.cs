using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Selectors;
using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.Orchestration.Runner.Domain.PipelineAggregate;

public class PipelineValidator : AbstractValidator<Pipeline>
{
    public PipelineValidator()
    {
        RuleFor(x => x.Id.ToString())
            .IsGuid();

        RuleFor(x => x.UserId.ToString())
            .IsGuid();

        RuleFor(x => x.Input)
            .NotNull();

        RuleForEach(x => x.Steps)
            .Must(x => x.Id.ToString().Trim().Length != 0)
            .WithMessage(x => "Pipeline id must not be empty")
            .Must(x => x.NodeId.Trim().Length != 0)
            .WithMessage(x => "Pipeline node id must not be empty")
            .Must(x => x.NodeProcedureId.Trim().Length != 0)
            .WithMessage(x => "Pipeline node procedure id must not be empty");

        RuleFor(x => x.Steps)
            .Must(x =>
            {
                for (int i = 0; i < x.Count - 1; i++)
                {
                    var stepA = x.ElementAt(i);
                    var stepB = x.ElementAt(i + 1);

                    var output = stepA.OutputSchema;
                    if (stepA.OutputValueSelector is not null)
                    {
                        var selectorResult = ChildElementSelector.FromString(stepA.OutputValueSelector);
                        if (selectorResult.IsFailure)
                            return false;
                        var selector = selectorResult.Data!;
                        var selectedValue = selector.Select(output);
                        if (selectedValue.IsFailure)
                            return false;

                        output = selectedValue.Data!;
                    }

                    var input = stepB.InputSchema;

                    var schemasCompatible = output.HasCompatibleSchemaWith(input);
                    if (!schemasCompatible)
                        return false;
                }

                return true;
            })
            .WithMessage(x => "Pipeline step schemas must be compatible");
    }
}