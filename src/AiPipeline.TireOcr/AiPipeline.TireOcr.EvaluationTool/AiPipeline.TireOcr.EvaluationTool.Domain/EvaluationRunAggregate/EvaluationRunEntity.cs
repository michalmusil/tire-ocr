using AiPipeline.TireOcr.EvaluationTool.Domain.Common;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.Evaluation;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.RunFailure;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

public class EvaluationRunEntity : TimestampedEntity
{
    public Guid Id { get; }
    public Guid? BatchId { get; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
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
        PostprocessingType postprocessingType, DbMatchingType dbMatchingType, string? title, string? description = null,
        Guid? batchId = null, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        BatchId = batchId;
        Title = title ?? Id.ToString();
        Description = description;
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

    public Result SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Trim().Length <= 2)
            return Result.Invalid("Evaluation run title must be at least 3 characters long");

        Title = title;
        return Result.Success();
    }

    public Result SetDescription(string? description)
    {
        Description = description;
        return Result.Success();
    }

    /// <summary>
    /// Determines a domain-specific category of the run's evaluation
    /// </summary>
    public EvaluationResultCategory GetEvaluationResultCategory()
    {
        var postprocessedTireCode = PostprocessingResult?.TireCode;
        if (RunFailure is not null)
            return RunFailure.Reason switch
            {
                EvaluationRunFailureReason.Preprocessing => EvaluationResultCategory.NoCodeDetectedPreprocessing,
                EvaluationRunFailureReason.Ocr => EvaluationResultCategory.NoCodeDetectedOcr,
                EvaluationRunFailureReason.Postprocessing => EvaluationResultCategory.NoCodeDetectedPostprocessing,
                EvaluationRunFailureReason.Unexpected => EvaluationResultCategory.NoCodeDetectedUnexpected,
                _ => throw new ArgumentOutOfRangeException()
            };
        if (postprocessedTireCode is null)
            return EvaluationResultCategory.NoCodeDetectedUnexpected;
        if (Evaluation is null)
            return EvaluationResultCategory.NoEvaluation;

        var mainParametersExtracted = postprocessedTireCode is
            { Width: not null, Diameter: not null, AspectRatio: not null, Construction: not null };
        if (!mainParametersExtracted)
            return EvaluationResultCategory.InsufficientExtraction;


        if (Evaluation.TotalDistance == 0)
            return EvaluationResultCategory.FullyCorrect;

        var mainParametersCorrect = Evaluation.WidthEvaluation?.Distance == 0 &&
                                    Evaluation.DiameterEvaluation?.Distance == 0 &&
                                    Evaluation.AspectRatioEvaluation?.Distance == 0 &&
                                    Evaluation.ConstructionEvaluation?.Distance == 0;
        var otherParametersNotIncorrect =
            ParameterNotExtractedOrMatchesExpectedValue(tc => tc.VehicleClass, e => e.VehicleClassEvaluation) &&
            ParameterNotExtractedOrMatchesExpectedValue(tc => tc.LoadRange, e => e.LoadRangeEvaluation) &&
            ParameterNotExtractedOrMatchesExpectedValue(tc => tc.LoadIndex, e => e.LoadIndexEvaluation) &&
            ParameterNotExtractedOrMatchesExpectedValue(tc => tc.LoadIndex2, e => e.LoadIndex2Evaluation) &&
            ParameterNotExtractedOrMatchesExpectedValue(tc => tc.SpeedRating, e => e.SpeedRatingEvaluation);
        if(mainParametersCorrect && otherParametersNotIncorrect)
            return EvaluationResultCategory.CorrectInMainParameters;

        return EvaluationResultCategory.FalsePositive;
    }

    private bool ParameterNotExtractedOrMatchesExpectedValue(
        Func<TireCodeValueObject, object?> parameterSelector,
        Func<EvaluationEntity, ParameterEvaluationValueObject?> evaluationSelector
    )
    {
        var postprocessedTireCode = PostprocessingResult?.TireCode;
        var evaluation = Evaluation;
        if (postprocessedTireCode is null || evaluation is null)
            return false;

        var parameterNotExtracted = parameterSelector(postprocessedTireCode) is null;
        var parameterMatchesExpectedValue = evaluationSelector(evaluation)?.Distance == 0;
        return parameterNotExtracted || parameterMatchesExpectedValue;
    }
}