using System.Globalization;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRunBatch.Dtos;

public record EvaluationRunCsvLineDto(
    string Id,
    string Title,
    string ResultCategory,
    string? Description,
    double? TotalDurationMs,
    string? InputImageUrl,
    string StartedAt,
    string? FinishedAt,
    string PreprocessingType,
    string OcrType,
    string PostprocessingType,
    string DbMatchingType,
    string? DetectedManufacturer,
    string? RawOcrResult,
    string? EstimatedCosts,
    string? PostprocessedResult,
    string? ExpectedResult,
    string? FirstDbMatchResult,
    decimal? VehicleClassDistance,
    decimal? WidthDistance,
    decimal? DiameterDistance,
    decimal? AspectRatioDistance,
    decimal? ConstructionDistance,
    decimal? LoadRangeDistance,
    decimal? LoadIndexDistance,
    decimal? LoadIndex2Distance,
    decimal? SpeedRatingDistance,
    string? FailureMessage
)
{
    public static EvaluationRunCsvLineDto FromDomain(EvaluationRunEntity domain)
    {
        return new(
            Id: domain.Id.ToString(),
            Title: domain.Title,
            ResultCategory: domain.GetEvaluationResultCategory().ToString(),
            Description: domain.Description,
            TotalDurationMs: domain.TotalExecutionDuration?.TotalMilliseconds,
            InputImageUrl: domain.InputImage.FileName,
            StartedAt: domain.StartedAt.ToString(CultureInfo.InvariantCulture),
            FinishedAt: domain.FinishedAt?.ToString(CultureInfo.InvariantCulture),
            PreprocessingType: domain.PreprocessingType.ToString(),
            OcrType: domain.OcrType.ToString(),
            PostprocessingType: domain.PostprocessingType.ToString(),
            DbMatchingType: domain.DbMatchingType.ToString(),
            DetectedManufacturer: domain.OcrResult?.DetectedManufacturer,
            RawOcrResult: domain.OcrResult?.DetectedCode,
            EstimatedCosts: domain.OcrResult?.EstimatedCost == null
                ? null
                : $"{decimal.Round(domain.OcrResult.EstimatedCost.Value, 5)}{domain.OcrResult.EstimatedCostCurrency ?? ""}",
            PostprocessedResult: domain.PostprocessingResult?.TireCode.ToString(),
            ExpectedResult: domain.Evaluation?.ExpectedTireCode.ToString(),
            FirstDbMatchResult: domain.DbMatchingResult?.Matches.FirstOrDefault()?.TireCode?.ToString(),
            VehicleClassDistance: domain.Evaluation?.VehicleClassEvaluation?.Distance,
            WidthDistance: domain.Evaluation?.WidthEvaluation?.Distance,
            DiameterDistance: domain.Evaluation?.DiameterEvaluation?.Distance,
            AspectRatioDistance: domain.Evaluation?.AspectRatioEvaluation?.Distance,
            ConstructionDistance: domain.Evaluation?.ConstructionEvaluation?.Distance,
            LoadRangeDistance: domain.Evaluation?.LoadRangeEvaluation?.Distance,
            LoadIndexDistance: domain.Evaluation?.LoadIndexEvaluation?.Distance,
            LoadIndex2Distance: domain.Evaluation?.LoadIndex2Evaluation?.Distance,
            SpeedRatingDistance: domain.Evaluation?.SpeedRatingEvaluation?.Distance,
            FailureMessage: domain.RunFailure?.Message
        );
    }
}