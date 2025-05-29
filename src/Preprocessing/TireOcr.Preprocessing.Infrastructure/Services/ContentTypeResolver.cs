using TireOcr.Preprocessing.Application.Services;

namespace TireOcr.Preprocessing.Infrastructure.Services;

public class ContentTypeResolver : IContentTypeResolver
{
    private static readonly List<string> SupportedContentTypes = ["image/jpeg", "image/png", "image/webp"];

    public bool IsContentTypeSupported(string contentType)
    {
        return SupportedContentTypes
            .Any(ct => string.Equals(ct, contentType, StringComparison.OrdinalIgnoreCase));
    }
}