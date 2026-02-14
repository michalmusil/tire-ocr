using Microsoft.Extensions.Configuration;
using TireOcr.Ocr.Application.Repositories;

namespace TireOcr.Ocr.Infrastructure.Repositories;

public class PromptRepositoryConfiguration : IPromptRepository
{
    private readonly IConfiguration _configuration;

    public PromptRepositoryConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> GetMainPromptAsync(bool useRandomPrefix = false)
    {
        var prompt = await GetOcrEnginePromptAsync();
        if (useRandomPrefix)
            prompt = PrependRandomString(prompt);
        return prompt;
    }

    public Task<string> GetBasePromptAsync() => Task.FromResult(GetPromptFromConfiguration("Base"));

    public Task<string> GetBaseStrictPromptAsync() => Task.FromResult(GetPromptFromConfiguration("BaseStrict"));

    public Task<string> GetPointsPromptAsync() => Task.FromResult(GetPromptFromConfiguration("Points"));

    public Task<string> GetPointsStrictPromptAsync() => Task.FromResult(GetPromptFromConfiguration("PointsStrict"));

    public Task<string> GetPointsStrictWithLisiContextPromptAsync() =>
        Task.FromResult(GetPromptFromConfiguration("PointsStrictLisiContext"));

    public Task<string> GetSpecializedDeepseekOcrPromptAsync() =>
        Task.FromResult(GetPromptFromConfiguration("DeepseekOcr"));

    public Task<string> GetOcrEnginePromptAsync() => Task.FromResult(GetPromptFromConfiguration("OcrEngine"));


    private string GetPromptFromConfiguration(string promptKey)
    {
        return _configuration.GetValue<string>($"Prompts:{promptKey}")!;
    }

    private string PrependRandomString(string prompt)
    {
        var randomNumber = new Random().Next(100000000, 999999999);
        var randomString = $"[Request ID: {randomNumber}]: {prompt}";
        return randomString;
    }
}