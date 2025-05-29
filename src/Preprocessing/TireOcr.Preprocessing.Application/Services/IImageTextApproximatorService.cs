using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Shared.Result;

namespace TireOcr.Preprocessing.Application.Services;

public interface IImageTextApproximatorService
{
    public DataResult<IEnumerable<string>> ApproximateStringsFromCharacters(
        IEnumerable<CharacterInImage> imageCharacters
    );

    public DataResult<Dictionary<string, int>> GetTireCodeLevenshteinDistanceOfStrings(IEnumerable<string> strings);
}