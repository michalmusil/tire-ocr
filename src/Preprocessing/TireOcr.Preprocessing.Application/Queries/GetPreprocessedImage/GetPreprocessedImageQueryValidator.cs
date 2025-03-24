using FluentValidation;

namespace TireOcr.Preprocessing.Application.Queries.GetPreprocessedImage;

public class GetPreprocessedImageQueryValidator : AbstractValidator<GetPreprocessedImageQuery>
{
    public GetPreprocessedImageQueryValidator()
    {
        RuleFor(q => q.ImageName)
            .NotEmpty();

        RuleFor(q => q.ImageData)
            .NotEmpty();
    }
}