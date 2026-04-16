using TireOcr.Shared.Domain;

namespace TireOcr.Preprocessing.Domain.Common;

public class StringInImage : ValueObject
{
    public required string RawString { get; init; }
    public required List<CharacterInImage> Characters { get; init; }

    protected override IEnumerable<object?> GetEqualityComponents() => [RawString, Characters];
}