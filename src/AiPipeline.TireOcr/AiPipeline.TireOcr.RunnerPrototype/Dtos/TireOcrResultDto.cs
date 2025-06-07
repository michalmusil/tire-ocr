using TireOcr.RunnerPrototype.Models;

namespace TireOcr.RunnerPrototype.Dtos;

public record TireOcrResultDto(
    string ImageFileName,
    TirePostprocessingResultDto TireCode,
    TireCodeDetectorType DetectorType,
    EstimatedCostsDto? EstimatedCosts,
    double TotalDurationMs,
    List<RunStatDto> RunTrace
);