using TireOcr.RunnerPrototype.Models;

namespace TireOcr.RunnerPrototype.Dtos;

public record TireOcrResult(
    string TireCode,
    TireCodeDetectorType DetectorType,
    List<RunStat> RunTrace
);