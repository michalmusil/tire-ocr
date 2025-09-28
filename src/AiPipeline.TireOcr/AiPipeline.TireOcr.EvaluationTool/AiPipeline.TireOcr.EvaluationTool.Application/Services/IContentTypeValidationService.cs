namespace AiPipeline.TireOcr.EvaluationTool.Application.Services;

public interface IContentTypeValidationService
{
    public bool IsContentTypeValid(string contentType);
    public string[] GetSupportedContentTypes();
}