using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Preprocessing.Domain.ImageEntity;

namespace TireOcr.Preprocessing.Application.Services;

public interface ITireSidewallExtractionService
{
    /// <summary>
    /// Unwraps circular tire sidewall into a straight strip from image based on circle boundary of the rim (rim = the entire wheel - tire sidewall)
    /// This unwrapping is performed based on experimentally discovered heuristics on how wide the
    /// tire sidewall is compared to rim radius.
    /// </summary>
    /// <param name="image">The image containing the entire wheel</param>
    /// <param name="rimCircle">Circle bounding the inner rim of the wheel (without tire sidewall)</param>
    /// <returns></returns>
    public Task<Image> ExtractSidewallStripAroundRimCircle(Image image, CircleInImage rimCircle);
}