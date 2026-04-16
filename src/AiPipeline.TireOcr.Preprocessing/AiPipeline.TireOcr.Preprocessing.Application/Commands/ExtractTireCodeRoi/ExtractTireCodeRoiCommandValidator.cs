using FluentValidation;

namespace TireOcr.Preprocessing.Application.Commands.ExtractTireCodeRoi;

public class ExtractTireCodeRoiCommandValidator : AbstractValidator<ExtractTireCodeRoiCommand>
{
    public ExtractTireCodeRoiCommandValidator()
    {
        RuleFor(q => q.ImageName)
            .NotEmpty();

        RuleFor(q => q.ImageData)
            .NotEmpty();
    }
}