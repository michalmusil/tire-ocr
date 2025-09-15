using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Shared.Result;

namespace TireOcr.Preprocessing.Application.Services;

public interface IImageTextApproximatorService
{
    public DataResult<IEnumerable<StringInImage>> ApproximateStringsFromCharacters(
        IEnumerable<CharacterInImage> imageCharacters
    );

    public DataResult<Dictionary<StringInImage, int>> GetTireCodeLevenshteinDistanceOfStrings(IEnumerable<StringInImage> strings);
}