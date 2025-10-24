using FluentValidation;

namespace TireOcr.Preprocessing.Application.Queries.GetImageSlices;

public class GetImageSlicesQueryValidator : AbstractValidator<GetImageSlicesQuery>
{
    public GetImageSlicesQueryValidator()
    {
        RuleFor(q => q.ImageName)
            .NotEmpty();

        RuleFor(q => q.ImageData)
            .NotEmpty();
        
        RuleFor(q => q.NumberOfSlices)
            .GreaterThan(0);
    }
}