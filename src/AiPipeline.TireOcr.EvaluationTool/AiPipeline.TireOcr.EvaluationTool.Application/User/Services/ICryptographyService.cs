namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Services;

public interface ICryptographyService
{
    public string GenerateCryptographicallyRandomString(int byteLength, bool urlSafe = true);
}