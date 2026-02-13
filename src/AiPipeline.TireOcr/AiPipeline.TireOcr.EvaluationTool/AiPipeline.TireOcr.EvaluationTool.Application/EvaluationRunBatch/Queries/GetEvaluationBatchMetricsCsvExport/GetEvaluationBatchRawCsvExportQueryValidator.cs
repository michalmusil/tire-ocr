using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Queries.GetEvaluationBatchMetricsCsvExport;

public class GetEvaluationBatchRawCsvExportQueryValidator : AbstractValidator<GetEvaluationBatchMetricsExportQuery>
{
    public GetEvaluationBatchRawCsvExportQueryValidator()
    {
        RuleFor(x => x.BatchId.ToString())
            .IsGuid();
        RuleFor(x => x.OtherBatchId.ToString())
            .IsGuidOrNull();
    }
}