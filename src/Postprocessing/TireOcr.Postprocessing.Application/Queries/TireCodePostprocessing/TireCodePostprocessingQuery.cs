using TireOcr.Postprocessing.Application.Dtos;
using TireOcr.Shared.UseCase;

namespace TireOcr.Postprocessing.Application.Queries.TireCodePostprocessing;

public record TireCodePostprocessingQuery(string RawTireCode) : IQuery<ProcessedTireCodeResultDto>;