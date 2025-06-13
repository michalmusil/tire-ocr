using AiPipeline.Orchestration.Contracts.Schema.Attributes;

namespace AiPipeline.Orchestration.Contracts.Schema.Properties;

[ApElementType("ApFile")]
public record ApFile : IApElement
{
    public string FileName { get; }
    public string FileUrl { get; }

    public ApFile(string fileName, string fileUrl)
    {
        FileName = fileName;
        FileUrl = fileUrl;
    }

    public bool HasEquivalentSchemaWith(IApElement other)
    {
        return other is ApFile;
    }
}