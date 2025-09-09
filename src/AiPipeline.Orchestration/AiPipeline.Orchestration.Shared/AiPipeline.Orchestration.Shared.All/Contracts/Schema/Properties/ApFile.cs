using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Attributes;

namespace AiPipeline.Orchestration.Shared.All.Contracts.Schema.Properties;

[ApElementType("ApFile")]
public record ApFile : IApElement
{
    public Guid Id { get; }
    public string ContentType { get; }
    public string[]? SupportedContentTypes { get; set; }

    public ApFile(Guid id, string contentType, string[]? supportedContentTypes = null)
    {
        Id = id;
        ContentType = contentType;
        SupportedContentTypes = supportedContentTypes;
    }

    public static ApFile Template(string[] supportedContentTypes) =>
        new(Guid.Empty, "", supportedContentTypes);

    public bool HasCompatibleSchemaWith(IApElement other)
    {
        if (other is not ApFile otherAsFile)
            return false;

        if (otherAsFile.ContentType != "")
        {
            var othersContentTypeMismatch = SupportedContentTypes?.Length > 0 && !SupportedContentTypes
                .Contains(otherAsFile.ContentType, StringComparer.OrdinalIgnoreCase);
            if (othersContentTypeMismatch)
                return false;
        }

        if (ContentType != "")
        {
            var thisContentTypeMismatch = otherAsFile.SupportedContentTypes?.Length > 0 &&
                                          !otherAsFile.SupportedContentTypes
                                              .Contains(ContentType, StringComparer.OrdinalIgnoreCase);
            if (thisContentTypeMismatch)
                return false;
        }

        return true;
    }

    public List<T> GetAllDescendantsOfType<T>() where T : IApElement => [];
}