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
        var prompt = await GetOcrEngineWithManufacturerPromptAsync();
        if (useRandomPrefix)
            prompt = PrependRandomString(prompt);
        return prompt;
    }

    public Task<string> GetSimplePromptAsync() => Task.FromResult(GetPromptFromConfiguration("Simple"));
    public Task<string> GetTaskPromptAsync() => Task.FromResult(GetPromptFromConfiguration("Task"));

    public Task<string> GetFewShotPromptAsync() => Task.FromResult(GetPromptFromConfiguration("FewShot"));
    public Task<string> GetCotPromptAsync()=> Task.FromResult(GetPromptFromConfiguration("Cot"));

    public Task<string> GetBasePromptAsync() => Task.FromResult(GetPromptFromConfiguration("Base"));

    public Task<string> GetBaseStrictPromptAsync() => Task.FromResult(GetPromptFromConfiguration("BaseStrict"));

    public Task<string> GetPointsPromptAsync() => Task.FromResult(GetPromptFromConfiguration("Points"));

    public Task<string> GetPointsStrictPromptAsync() => Task.FromResult(GetPromptFromConfiguration("PointsStrict"));

    public Task<string> GetPointsStrictWithLisiContextPromptAsync() =>
        Task.FromResult(GetPromptFromConfiguration("PointsStrictLisiContext"));

    public Task<string> GetSpecializedDeepseekOcrPromptAsync() =>
        Task.FromResult(GetPromptFromConfiguration("DeepseekOcr"));

    public Task<string> GetSpecializedHunyuanOcrPromptAsync() =>
        Task.FromResult(GetPromptFromConfiguration("HunyuanOcr"));

    public Task<string> GetOcrEnginePromptAsync() => Task.FromResult(GetPromptFromConfiguration("OcrEngine"));
    
    private Task<string> GetOcrEngineWithManufacturerPromptAsync() => Task.FromResult(GetPromptFromConfiguration("OcrEngineWithManufacturer"));


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