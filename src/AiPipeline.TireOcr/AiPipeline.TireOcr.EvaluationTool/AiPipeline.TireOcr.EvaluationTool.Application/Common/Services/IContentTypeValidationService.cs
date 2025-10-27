using System.Collections.ObjectModel;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Common.Services;

public interface IContentTypeValidationService
{
    public bool IsContentTypeValid(string contentType);
    public ReadOnlyCollection<string> GetSupportedContentTypes();
}