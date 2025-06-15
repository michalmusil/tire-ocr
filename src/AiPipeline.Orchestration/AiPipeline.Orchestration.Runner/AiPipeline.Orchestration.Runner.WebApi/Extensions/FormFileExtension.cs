namespace AiPipeline.Orchestration.Runner.WebApi.Extensions;

public static class FormFileExtension
{
    public static async Task<byte[]> ToByteArray(this IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}