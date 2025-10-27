using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Services;

public interface IHashService
{
    public DataResult<string> GetHashOf(string value);
    public Result Verify(string value, string hashedValue);
}