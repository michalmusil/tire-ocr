using TireOcr.RunnerPrototype.Models;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Dtos.Preprocessing;

public record ImageDownloadResultDto(string ImageUrl, DataResult<Image> Result);