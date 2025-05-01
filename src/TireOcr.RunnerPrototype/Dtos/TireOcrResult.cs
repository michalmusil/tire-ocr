using TireOcr.RunnerPrototype.Models;

namespace TireOcr.RunnerPrototype.Dtos;

public record TireOcrResult(
    string ImageFileName,
    TirePostprocessingResult TireCode,
    TireCodeDetectorType DetectorType,
    EstimatedCostsDto? EstimatedCosts,
    double TotalDurationMs,
    List<RunStat> RunTrace
);