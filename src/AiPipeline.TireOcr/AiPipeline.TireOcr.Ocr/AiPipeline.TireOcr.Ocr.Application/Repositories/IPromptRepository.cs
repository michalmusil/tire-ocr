namespace TireOcr.Ocr.Application.Repositories;

public interface IPromptRepository
{
    public Task<string> GetMainPromptAsync(bool useRandomPrefix = false);
    public Task<string> GetBasePromptAsync();
    public Task<string> GetBaseStrictPromptAsync();
    public Task<string> GetPointsPromptAsync();
    public Task<string> GetPointsStrictPromptAsync();
    public Task<string> GetPointsStrictWithLisiContextPromptAsync();
    public Task<string> GetSpecializedDeepseekOcrPromptAsync();
    public Task<string> GetSpecializedHunyuanOcrPromptAsync();
    public Task<string> GetOcrEnginePromptAsync();
}