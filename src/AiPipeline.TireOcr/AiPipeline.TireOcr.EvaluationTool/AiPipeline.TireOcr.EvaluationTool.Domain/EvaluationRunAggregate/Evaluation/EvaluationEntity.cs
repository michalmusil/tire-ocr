using AiPipeline.TireOcr.EvaluationTool.Domain.Common;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.Evaluation;

public class EvaluationEntity : TimestampedEntity
{
    public Guid Id { get; }
    public Guid RunId { get; private set; }
    public TireCodeValueObject ExpectedTireCode { get; }
    public int TotalDistance { get; }
    public int FullMatchParameterCount { get; }
    public decimal EstimatedAccuracy { get; }
    public ParameterEvaluationValueObject? VehicleClassEvaluation { get; }
    public ParameterEvaluationValueObject? WidthEvaluation { get; }
    public ParameterEvaluationValueObject? DiameterEvaluation { get; }
    public ParameterEvaluationValueObject? AspectRatioEvaluation { get; }
    public ParameterEvaluationValueObject? ConstructionEvaluation { get; }
    public ParameterEvaluationValueObject? LoadRangeEvaluation { get; }
    public ParameterEvaluationValueObject? LoadIndexEvaluation { get; }
    public ParameterEvaluationValueObject? LoadIndex2Evaluation { get; }
    public ParameterEvaluationValueObject? SpeedRatingEvaluation { get; }

    private EvaluationEntity()
    {
    }

    public EvaluationEntity(Guid evaluationRunId, TireCodeValueObject expectedTireCode, int totalDistance,
        int fullMatchParameterCount, decimal estimatedAccuracy, ParameterEvaluationValueObject? vehicleClassEvaluation,
        ParameterEvaluationValueObject? widthEvaluation, ParameterEvaluationValueObject? diameterEvaluation,
        ParameterEvaluationValueObject? aspectRatioEvaluation, ParameterEvaluationValueObject? constructionEvaluation,
        ParameterEvaluationValueObject? loadRangeEvaluation, ParameterEvaluationValueObject? loadIndexEvaluation,
        ParameterEvaluationValueObject? loadIndex2Evaluation, ParameterEvaluationValueObject? speedRatingEvaluation,
        Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        RunId = evaluationRunId;
        ExpectedTireCode = expectedTireCode;
        TotalDistance = totalDistance;
        FullMatchParameterCount = fullMatchParameterCount;
        EstimatedAccuracy = estimatedAccuracy;
        VehicleClassEvaluation = vehicleClassEvaluation;
        WidthEvaluation = widthEvaluation;
        DiameterEvaluation = diameterEvaluation;
        AspectRatioEvaluation = aspectRatioEvaluation;
        ConstructionEvaluation = constructionEvaluation;
        LoadRangeEvaluation = loadRangeEvaluation;
        LoadIndexEvaluation = loadIndexEvaluation;
        LoadIndex2Evaluation = loadIndex2Evaluation;
        SpeedRatingEvaluation = speedRatingEvaluation;
    }
    
    public void SetEvaluationRunId(Guid evaluationRunId)
    {
        RunId = evaluationRunId;
        SetUpdated();
    }
}