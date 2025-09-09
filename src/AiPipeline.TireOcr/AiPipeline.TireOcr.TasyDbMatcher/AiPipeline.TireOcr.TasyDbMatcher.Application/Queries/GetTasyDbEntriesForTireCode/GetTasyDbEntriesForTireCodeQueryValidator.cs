using FluentValidation;

namespace AiPipeline.TireOcr.TasyDbMatcher.Application.Queries.GetTasyDbEntriesForTireCode;

public class GetTasyDbEntriesForTireCodeQueryValidator : AbstractValidator<GetTasyDbEntriesForTireCodeQuery>
{
    public GetTasyDbEntriesForTireCodeQueryValidator()
    {
        RuleFor(x => x.DetectedCode)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.DetectedCode.PostprocessedTireCode)
            .NotNull()
            .NotEmpty();
    }
}