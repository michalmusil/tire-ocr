using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Shared.Result;

namespace TireOcr.Preprocessing.Application.Services;

public interface IImageTextApproximator
{
    public DataResult<IEnumerable<string>> Approximate(IEnumerable<CharacterInImage> imageCharacters);
}