using System.Security.Cryptography;
using AiPipeline.Orchestration.Runner.Application.User.Services;

namespace AiPipeline.Orchestration.Runner.Infrastructure.User.Services;

public class CryptographyService : ICryptographyService
{
    public string GenerateCryptographicallyRandomString(int byteLength, bool urlSafe = true)
    {
        var randNum = new byte[byteLength];
        using (var numberGenerator = RandomNumberGenerator.Create())
        {
            numberGenerator.GetBytes(randNum);
        }

        var randString = Convert.ToBase64String(randNum);
        if (urlSafe)
        {
            randString = randString
                .Replace("/", "_")
                .Replace("+", "-");
        }

        return randString;
    }
}