using FluentValidation;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Commands.RunSingleEvaluation;

public class RunSingleEvaluationCommandValidator : AbstractValidator<RunSingleEvaluationCommand>
{
    public RunSingleEvaluationCommandValidator()
    {
        string[] supportedContentTypes = ["image/jpeg", "image/png", "image/webp"];

        RuleFor(c => c.InputImage)
            .NotNull()
            .Must(ii => ii.ImageData.Length > 0)
            .WithMessage("Image data is empty")
            .Must(ii => supportedContentTypes.Contains(ii.ContentType, StringComparer.OrdinalIgnoreCase))
            .WithMessage(
                $"Image content type is not supported. Content type must be one of: {string.Join(',', supportedContentTypes)}");
    }
}