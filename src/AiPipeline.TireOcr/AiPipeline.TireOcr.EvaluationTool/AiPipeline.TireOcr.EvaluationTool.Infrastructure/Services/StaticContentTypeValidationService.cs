using System.Collections.ObjectModel;
using AiPipeline.TireOcr.EvaluationTool.Application.Services;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Services;

public class StaticContentTypeValidationService : IContentTypeValidationService
{
    private readonly string[] _supportedContentTypes = ["image/jpeg", "image/png", "image/webp"];

    public bool IsContentTypeValid(string contentType) =>
        _supportedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase);

    public ReadOnlyCollection<string> GetSupportedContentTypes() => _supportedContentTypes.AsReadOnly();
}