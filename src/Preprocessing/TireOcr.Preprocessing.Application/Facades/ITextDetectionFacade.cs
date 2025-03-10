using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Shared.Result;

namespace TireOcr.Preprocessing.Application.Facades;

public interface ITextDetectionFacade
{
    public Task<DataResult<TextDetectionResult>> GetTextAreaFromImageAsync(Image image);
}