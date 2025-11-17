using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun.DbMatching;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun.Evaluation;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun.Ocr;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun;

public record EvaluationRunDto(
    string Id,
    string Title,
    string? Description,
    EvaluationResultCategory EvaluationResultCategory,
    ImageInfoDto InputImage,
    DateTime StartedAt,
    DateTime? FinishedAt,
    RunConfigDto RunConfig,
    EvaluationRunFailureDto? Failure,
    EvaluationDto? Evaluation,
    PreprocessingResultDto? PreprocessingResult,
    OcrResultDto? OcrResult,
    PostprocessingResultDto? PostprocessingResult,
    DbMatchingResultDto? DbMatchingResult,
    long? TotalExecutionDurationMs
)
{
    public static EvaluationRunDto FromDomain(Domain.EvaluationRunAggregate.EvaluationRunEntity domain)
    {
        return new
        (
            Id: domain.Id.ToString(),
            Title: domain.Title,
            Description: domain.Description,
            EvaluationResultCategory: domain.GetEvaluationResultCategory(),
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
            Evaluation: domain.Evaluation is null ? null : EvaluationDto.FromDomain(domain.Evaluation),
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