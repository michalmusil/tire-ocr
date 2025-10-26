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

    public Task<string> GetMainPromptAsync() => GetPointsStrictPromptAsync();

    public Task<string> GetBasePromptAsync() => Task.FromResult(GetPromptFromConfiguration("Base"));

    public Task<string> GetBaseStrictPromptAsync() => Task.FromResult(GetPromptFromConfiguration("BaseStrict"));

    public Task<string> GetPointsPromptAsync() => Task.FromResult(GetPromptFromConfiguration("Points"));

    public Task<string> GetPointsStrictPromptAsync() => Task.FromResult(GetPromptFromConfiguration("PointsStrict"));

    public Task<string> GetPointsStrictWithLisiContextPromptAsync() =>
        Task.FromResult(GetPromptFromConfiguration("PointsStrictLisiContext"));

    private string GetPromptFromConfiguration(string promptKey)
    {
        return _configuration.GetValue<string>($"Prompts:{promptKey}")!;
    }
}