using FluentValidation;

namespace TireOcr.Ocr.Application.Queries.PerformTireImageOcr;

public class PerformTireImageOcrQueryValidator: AbstractValidator<PerformTireImageOcrQuery>
{
    public PerformTireImageOcrQueryValidator()
    {
        RuleFor(x => x.ImageData)
            .NotEmpty();
        
        RuleFor(x => x.ImageName)
            .NotEmpty();
        
        RuleFor(x => x.ImageData)
            .NotEmpty();
    }
}