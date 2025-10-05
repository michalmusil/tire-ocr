using AiPipeline.TireOcr.EvaluationTool.Domain.Common;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.Evaluation;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.RunFailure;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

public class EvaluationRunEntity : TimestampedEntity
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

    public EvaluationEntity? Evaluation { get; private set; }

    public PreprocessingResultEntity? PreprocessingResult { get; private set; }
    public OcrResultEntity? OcrResult { get; private set; }
    public PostprocessingResultEntity? PostprocessingResult { get; private set; }
    public DbMatchingResultEntity? DbMatchingResult { get; private set; }

    public bool HasFinished => FinishedAt.HasValue;
    public bool HasRunFailed => RunFailure is not null;

    public TimeSpan? TotalExecutionDuration => FinishedAt.HasValue ? FinishedAt.Value - StartedAt : null;

    private EvaluationRunEntity()
    {
    }

    public EvaluationRunEntity(ImageValueObject inputImage, PreprocessingType preprocessingType, OcrType ocrType,
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

    public void SetEvaluation(EvaluationEntity evaluation)
    {
        Evaluation = evaluation;
        SetFinishedAt(DateTime.UtcNow);
    }

    public void SetPreprocessingResult(PreprocessingResultEntity preprocessingResult)
    {
        PreprocessingResult = preprocessingResult;
        SetUpdated();
    }

    public void SetOcrResult(OcrResultEntity ocrResult)
    {
        OcrResult = ocrResult;
        SetUpdated();
    }


    public void SetPostprocessingResult(PostprocessingResultEntity postprocessingResult)
    {
        PostprocessingResult = postprocessingResult;
        if (DbMatchingType is DbMatchingType.None)
            FinishedAt = DateTime.UtcNow;
        SetUpdated();
    }

    public void SetDbMatchingResult(DbMatchingResultEntity dbMatchingResult)
    {
        DbMatchingResult = dbMatchingResult;
        SetFinishedAt(DateTime.UtcNow);
    }

    public void SetFinishedAt(DateTime finishedAt)
    {
        FinishedAt = finishedAt.ToUniversalTime();
        SetUpdated();
    }
}