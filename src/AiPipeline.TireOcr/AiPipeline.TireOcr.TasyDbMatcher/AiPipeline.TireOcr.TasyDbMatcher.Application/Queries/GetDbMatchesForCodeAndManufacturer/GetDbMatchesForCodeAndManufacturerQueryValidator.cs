using FluentValidation;

namespace AiPipeline.TireOcr.TasyDbMatcher.Application.Queries.GetDbMatchesForCodeAndManufacturer;

public class GetDbMatchesForCodeAndManufacturerQueryValidator : AbstractValidator<GetDbMatchesForCodeAndManufacturerQuery>
{
    public GetDbMatchesForCodeAndManufacturerQueryValidator()
    {
        RuleFor(x => x.DetectedCode)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.DetectedCode.PostprocessedTireCode)
            .NotNull()
            .NotEmpty();
    }
}