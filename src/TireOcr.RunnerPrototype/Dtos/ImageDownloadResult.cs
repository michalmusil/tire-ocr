using TireOcr.RunnerPrototype.Models;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Dtos;

public record ImageDownloadResult(string ImageUrl, DataResult<Image> Result);