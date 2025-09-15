using FluentValidation;

namespace TireOcr.Preprocessing.Application.Queries.GetTireCodeRoi;

public class GetTireCodeRoiQueryValidator : AbstractValidator<GetTireCodeRoiQuery>
{
    public GetTireCodeRoiQueryValidator()
    {
        RuleFor(q => q.ImageName)
            .NotEmpty();

        RuleFor(q => q.ImageData)
            .NotEmpty();
    }
}