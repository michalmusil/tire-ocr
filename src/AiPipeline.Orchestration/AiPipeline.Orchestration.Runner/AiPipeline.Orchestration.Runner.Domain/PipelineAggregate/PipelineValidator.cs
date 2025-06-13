using FluentValidation;

namespace AiPipeline.Orchestration.Runner.Domain.PipelineAggregate;

public class PipelineValidator : AbstractValidator<Pipeline>
{
    public PipelineValidator()
    {
        RuleFor(x => x.Id.ToString())
            .NotEmpty();

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

                    var schemasCompatible = stepA.OutputSchema.HasCompatibleSchemaWith(stepB.InputSchema);
                    if (!schemasCompatible)
                        return false;
                }

                return true;
            })
            .WithMessage(x => "Pipeline step schemas must be compatible");
    }
}