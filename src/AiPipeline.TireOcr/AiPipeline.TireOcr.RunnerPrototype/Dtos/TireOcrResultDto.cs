using TireOcr.RunnerPrototype.Dtos.DbMatching;
using TireOcr.RunnerPrototype.Dtos.Ocr;
using TireOcr.RunnerPrototype.Dtos.Postprocessing;
using TireOcr.RunnerPrototype.Models;

namespace TireOcr.RunnerPrototype.Dtos;

public record TireOcrResultDto(
    string ImageFileName,
    TireCodeDetectorType DetectorType,
    OcrServiceResultDto OcrResult,
    TirePostprocessingResultDto PostprocessingResult,
    DbMatcherServiceResponseDto TasyDbMatchesResult,
    double TotalDurationMs,
    List<RunStatDto> RunTrace
);