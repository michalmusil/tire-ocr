using TireOcr.RunnerPrototype.Models;

namespace TireOcr.RunnerPrototype.Dtos;

public record TireOcrResult(
    TirePostprocessingResult TireCode,
    TireCodeDetectorType DetectorType,
    EstimatedCostsDto EstimatedCosts,
    List<RunStat> RunTrace
);