using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun.DbMatching;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun.Ocr;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;

public record EvaluationRunDto(
    string Id,
    ImageInfoDto InputImage,
    DateTime StartedAt,
    DateTime? FinishedAt,
    RunConfigDto RunConfig,
    EvaluationRunFailureDto? Failure,
    TireCodeDto? ExpectedPostprocessingResult,
    PreprocessingResultDto? PreprocessingResult,
    OcrResultDto? OcrResult,
    PostprocessingResultDto? PostprocessingResult,
    DbMatchingResultDto? DbMatchingResult,
    long? TotalExecutionDurationMs
)
{
    public static EvaluationRunDto FromDomain(Domain.EvaluationRunAggregate.EvaluationRun domain)
    {
        return new
        (
            Id: domain.Id.ToString(),
            InputImage: ImageInfoDto.FromDomain(domain.InputImage),
            StartedAt: domain.StartedAt,
            FinishedAt: domain.FinishedAt,
            Failure: domain.RunFailure is null
                ? null
                : new EvaluationRunFailureDto(
                    FailureReason: domain.RunFailure.Reason.ToString(),
                    Code: domain.RunFailure.Code,
                    Message: domain.RunFailure.Message
                ),
            TotalExecutionDurationMs: (long?)domain.TotalExecutionDuration?.TotalMilliseconds,
            RunConfig: new(
                PreprocessingType: domain.PreprocessingType,
                OcrType: domain.OcrType,
                PostprocessingType: domain.PostprocessingType,
                DbMatchingType: domain.DbMatchingType
            ),
            ExpectedPostprocessingResult: domain.ExpectedPostprocessingResult is null
                ? null
                : TireCodeDto.FromDomain(domain.ExpectedPostprocessingResult),
            PreprocessingResult: domain.PreprocessingResult is null
                ? null
                : PreprocessingResultDto.FromDomain(domain.PreprocessingResult),
            OcrResult: domain.OcrResult is null ? null : OcrResultDto.FromDomain(domain.OcrResult),
            PostprocessingResult: domain.PostprocessingResult is null
                ? null
                : PostprocessingResultDto.FromDomain(domain.PostprocessingResult),
            DbMatchingResult: domain.DbMatchingResult is null
                ? null
                : DbMatchingResultDto.FromDomain(domain.DbMatchingResult)
        );
    }
}