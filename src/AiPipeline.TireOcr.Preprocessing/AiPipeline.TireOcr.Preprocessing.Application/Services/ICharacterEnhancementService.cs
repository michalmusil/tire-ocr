using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Shared.Result;

namespace TireOcr.Preprocessing.Application.Services;

public interface ICharacterEnhancementService
{
    public Task<DataResult<Image>> EnhanceCharactersAsync(Image image);
}