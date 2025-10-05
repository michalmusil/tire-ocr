using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.Evaluation;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.RunFailure;
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

    public EvaluationRunFailureValueObject? RunFailure { get; private set; }

    public EvaluationValueObject? Evaluation { get; private set; }

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
        PostprocessingType postprocessingType, DbMatchingType dbMatchingType, string? title, Guid? id = null)
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
        Evaluation = null;
        PreprocessingResult = null;
        OcrResult = null;
        PostprocessingResult = null;
        DbMatchingResult = null;
    }

    public void SetFailure(EvaluationRunFailureValueObject failureValueObject)
    {
        RunFailure = failureValueObject;
        SetFinishedAt(DateTime.UtcNow);
    }

    public void SetEvaluation(EvaluationValueObject evaluation)
    {
        Evaluation = evaluation;
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