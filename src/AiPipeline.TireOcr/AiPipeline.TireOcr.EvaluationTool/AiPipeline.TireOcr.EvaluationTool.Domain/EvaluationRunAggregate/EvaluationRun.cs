using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

public class EvaluationRun
{
    public Guid Id { get; }
    public string Title { get; }
    public ImageValueObject InputImage { get; }
    public DateTime StartedAt { get; }
    public DateTime? FinishedAt { get; private set; }

    public PreprocessingType PreprocessingType { get; }
    public OcrType OcrType { get; }
    public PostprocessingType PostprocessingType { get; }
    public DbMatchingType DbMatchingType { get; }

    public EvaluationRunFailure? RunFailure { get; private set; }

    public TireCodeValueObject? ExpectedPostprocessingResult { get; }

    public PreprocessingResultValueObject? PreprocessingResult { get; private set; }
    public OcrResultValueObject? OcrResult { get; private set; }
    public PostprocessingResultValueObject? PostprocessingResult { get; private set; }
    public DbMatchingResultValueObject? DbMatchingResult { get; private set; }

    public bool HasFinished => FinishedAt.HasValue;
    public bool HasRunFailed => RunFailure is not null;

    public TimeSpan? TotalExecutionDuration => FinishedAt.HasValue ? FinishedAt.Value - StartedAt : null;

    private EvaluationRun()
    {
    }

    public EvaluationRun(ImageValueObject inputImage, PreprocessingType preprocessingType, OcrType ocrType,
        PostprocessingType postprocessingType, DbMatchingType dbMatchingType, string? title,
        TireCodeValueObject? expectedPostprocessingResult, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        Title = title ?? Id.ToString();
        InputImage = inputImage;
        StartedAt = DateTime.UtcNow;
        FinishedAt = null;
        RunFailure = null;
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

    public void SetFailure(EvaluationRunFailure failure)
    {
        RunFailure = failure;
        SetFinishedAt(DateTime.UtcNow);
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

    public void SetFinishedAt(DateTime finishedAt) => FinishedAt = finishedAt.ToUniversalTime();
}