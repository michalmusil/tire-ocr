using TireOcr.RunnerPrototype.Models;

namespace TireOcr.RunnerPrototype.Dtos;

public record TireOcrResultDto(
    string ImageFileName,
    TireCodeDetectorType DetectorType,
    OcrServiceResultDto OcrResult,
    TirePostprocessingResultDto PostprocessingResult,
    double TotalDurationMs,
    List<RunStatDto> RunTrace
);