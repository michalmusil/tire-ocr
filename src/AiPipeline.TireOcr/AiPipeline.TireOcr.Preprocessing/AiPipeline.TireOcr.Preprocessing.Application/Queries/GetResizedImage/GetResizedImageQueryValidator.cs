using FluentValidation;

namespace TireOcr.Preprocessing.Application.Queries.GetResizedImage;

public class GetResizedImageQueryValidator : AbstractValidator<GetResizedImageQuery>
{
    public GetResizedImageQueryValidator()
    {
        RuleFor(q => q.ImageName)
            .NotEmpty();

        RuleFor(q => q.ImageData)
            .NotEmpty();

        RuleFor(q => q.MaxImageSideDimension)
            .GreaterThanOrEqualTo(300)
            .LessThanOrEqualTo(8192);
    }
}