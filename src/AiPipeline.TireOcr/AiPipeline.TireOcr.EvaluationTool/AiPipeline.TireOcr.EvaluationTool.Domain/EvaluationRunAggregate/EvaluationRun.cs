using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

public class EvaluationRun
{
    public Guid Id { get; }
    public ImageValueObject InputImage { get; }
    public DateTime StartedAt { get; }
    public DateTime? FinishedAt { get; private set; }

    public PreprocessingType PreprocessingType { get; }
    public OcrType OcrType { get; }
    public PostprocessingType PostprocessingType { get; }
    public DbMatchingType DbMatchingType { get; }

    public TireCodeValueObject? ExpectedPostprocessingResult { get; }

    public PreprocessingResultValueObject? PreprocessingResult { get; private set; }
    public OcrResultValueObject? OcrResult { get; private set; }
    public PostprocessingResultValueObject? PostprocessingResult { get; private set; }
    public DbMatchingResultValueObject? DbMatchingResult { get; private set; }

    public bool HasFinished => FinishedAt.HasValue;
    public bool HasRunFailed => FinishedAt.HasValue && PostprocessingResult is null;

    public TimeSpan? TotalExecutionDuration => FinishedAt.HasValue ? FinishedAt.Value - StartedAt : null;

    public EvaluationRunFailureReason? FailureReason => HasRunFailed
        ? null
        : PreprocessingType is not PreprocessingType.None && PreprocessingResult is null
            ? EvaluationRunFailureReason.Preprocessing
            : OcrResult is null
                ? EvaluationRunFailureReason.Ocr
                : PostprocessingResult is null
                    ? EvaluationRunFailureReason.Postprocessing
                    : EvaluationRunFailureReason.Unexpected;

    private EvaluationRun()
    {
    }

    public EvaluationRun(ImageValueObject inputImage, PreprocessingType preprocessingType, OcrType ocrType,
        PostprocessingType postprocessingType, DbMatchingType dbMatchingType,
        TireCodeValueObject? expectedPostprocessingResult, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        InputImage = inputImage;
        StartedAt = DateTime.UtcNow;
        FinishedAt = null;

        PreprocessingType = preprocessingType;
        OcrType = ocrType;
        PostprocessingType = postprocessingType;
        DbMatchingType = dbMatchingType;

        ExpectedPostprocessingResult = expectedPostprocessingResult;
        PreprocessingResult = null;
        OcrResult = null;
        PostprocessingResult = null;
        DbMatchingResult = null;
    }

    public void SetPreprocessingResult(PreprocessingResultValueObject preprocessingResult) =>
        PreprocessingResult = preprocessingResult;

    public void SetOcrResult(OcrResultValueObject ocrResult) => OcrResult = ocrResult;

    public void SetPostprocessingResult(PostprocessingResultValueObject postprocessingResult)
    {
        PostprocessingResult = postprocessingResult;
        if (DbMatchingType is DbMatchingType.None)
            FinishedAt = DateTime.UtcNow;
    }

    public void SetDbMatchingResult(DbMatchingResultValueObject dbMatchingResult)
    {
        DbMatchingResult = dbMatchingResult;
        FinishedAt = DateTime.UtcNow;
    }
}