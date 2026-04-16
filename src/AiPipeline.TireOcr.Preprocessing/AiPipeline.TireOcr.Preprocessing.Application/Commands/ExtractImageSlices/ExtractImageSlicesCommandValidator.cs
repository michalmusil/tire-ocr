using FluentValidation;

namespace TireOcr.Preprocessing.Application.Commands.ExtractImageSlices;

public class ExtractImageSlicesCommandValidator : AbstractValidator<ExtractImageSlicesCommand>
{
    public ExtractImageSlicesCommandValidator()
    {
        RuleFor(q => q.ImageName)
            .NotEmpty();

        RuleFor(q => q.ImageData)
            .NotEmpty();
        
        RuleFor(q => q.NumberOfSlices)
            .GreaterThan(0);
    }
}