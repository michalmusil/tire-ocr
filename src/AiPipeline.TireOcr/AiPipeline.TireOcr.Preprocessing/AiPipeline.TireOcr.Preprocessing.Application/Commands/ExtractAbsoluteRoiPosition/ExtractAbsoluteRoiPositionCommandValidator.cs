using FluentValidation;

namespace TireOcr.Preprocessing.Application.Commands.ExtractAbsoluteRoiPosition;

public class ExtractAbsoluteRoiPositionCommandValidator : AbstractValidator<ExtractAbsoluteRoiPositionCommand>
{
    public ExtractAbsoluteRoiPositionCommandValidator()
    {
        RuleFor(q => q.ImageName)
            .NotEmpty();

        RuleFor(q => q.ImageData)
            .NotEmpty();
    }
}