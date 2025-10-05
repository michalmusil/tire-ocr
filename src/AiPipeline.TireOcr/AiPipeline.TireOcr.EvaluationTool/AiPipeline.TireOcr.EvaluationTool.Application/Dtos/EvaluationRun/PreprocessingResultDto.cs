using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;

public record PreprocessingResultDto(
    ImageInfoDto PreprocessedImage,
    long DurationMs
)
{
    public static PreprocessingResultDto FromDomain(PreprocessingResultEntity domain) => new(
        PreprocessedImage: ImageInfoDto.FromDomain(domain.PreprocessingResult),
        DurationMs: domain.DurationMs
    );
}