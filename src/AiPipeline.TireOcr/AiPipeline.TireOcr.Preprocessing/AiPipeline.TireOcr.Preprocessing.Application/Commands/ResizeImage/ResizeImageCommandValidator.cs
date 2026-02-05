using FluentValidation;

namespace TireOcr.Preprocessing.Application.Commands.ResizeImage;

public class ResizeImageCommandValidator : AbstractValidator<ResizeImageCommand>
{
    public ResizeImageCommandValidator()
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