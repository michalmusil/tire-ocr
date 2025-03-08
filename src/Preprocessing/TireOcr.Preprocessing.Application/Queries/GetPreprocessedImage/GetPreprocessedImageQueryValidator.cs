using FluentValidation;

namespace TireOcr.Preprocessing.Application.Queries.GetPreprocessedImage;

public class GetPreprocessedImageQueryValidator : AbstractValidator<GetPreprocessedImageQuery>
{
    public GetPreprocessedImageQueryValidator()
    {
        RuleFor(q => q.ImageName)
            .NotNull()
            .NotEmpty();

        RuleFor(q => q.ImageData)
            .NotNull()
            .NotEmpty();
    }
}