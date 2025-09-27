using TireOcr.Shared.Domain;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

public class ImageValueObject : ValueObject
{
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    protected override IEnumerable<object?> GetEqualityComponents() => [FileName, ContentType];
}