using AiPipeline.TireOcr.EvaluationTool.Application.Services;
using FluentValidation;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Commands.RunSingleEvaluation;

public class RunSingleEvaluationCommandValidator : AbstractValidator<RunSingleEvaluationCommand>
{
    public RunSingleEvaluationCommandValidator(IContentTypeValidationService contentTypeValidationService)
    {
        RuleFor(c => c)
            .Must(c => c.InputImageUrl != null || c.InputImage != null)
            .WithMessage("Either input image data or input image url must be provided");

        RuleFor(c => c.InputImage)
            .Must(ii => ii is null || ii.ImageData.Length > 0)
            .WithMessage("Image data is empty")
            .Must(ii => ii is null || contentTypeValidationService.IsContentTypeValid(ii.ContentType))
            .WithMessage(
                $"Image content type is not supported. Content type must be one of: {string.Join(',', contentTypeValidationService.GetSupportedContentTypes())}");

        RuleFor(c => c.RunConfig)
            .NotNull();
    }
}