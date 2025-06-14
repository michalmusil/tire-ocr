using AiPipeline.Orchestration.Contracts.Schema.Attributes;

namespace AiPipeline.Orchestration.Contracts.Schema.Properties;

[ApElementType("ApFile")]
public record ApFile : IApElement
{
    public string FileName { get; }
    public string ContentType { get; }
    public string FileUrl { get; }

    public ApFile(string fileName, string contentType, string fileUrl)
    {
        FileName = fileName;
        ContentType = contentType;
        FileUrl = fileUrl;
    }

    public bool HasCompatibleSchemaWith(IApElement other)
    {
        return other is ApFile;
    }
    
    public List<T> GetAllDescendantsOfType<T>() where T : IApElement => [];
}