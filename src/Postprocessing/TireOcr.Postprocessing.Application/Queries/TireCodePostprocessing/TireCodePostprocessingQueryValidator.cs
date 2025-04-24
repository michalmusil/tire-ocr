using FluentValidation;

namespace TireOcr.Postprocessing.Application.Queries.TireCodePostprocessing;

public class TireCodePostprocessingQueryValidator : AbstractValidator<TireCodePostprocessingQuery>
{
    public TireCodePostprocessingQueryValidator()
    {
        RuleFor(x => x.RawTireCode)
            .NotEmpty();
    }
}