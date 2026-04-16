using AiPipeline.TireOcr.EvaluationTool.Application.User.Services;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.User.Services;

public class HashService : IHashService
{
    private const int WorkFactor = 13;

    public DataResult<string> GetHashOf(string value)
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(value, WorkFactor);
        if (hash is null)
            return DataResult<string>.Failure(new Failure(500, "Failed to hash value"));

        return DataResult<string>.Success(hash);
    }

    public Result Verify(string value, string hashedValue)
    {
        var isValid = BCrypt.Net.BCrypt.Verify(value, hashedValue);

        return isValid ? Result.Success() : Result.Invalid("Value is not valid");
    }
}