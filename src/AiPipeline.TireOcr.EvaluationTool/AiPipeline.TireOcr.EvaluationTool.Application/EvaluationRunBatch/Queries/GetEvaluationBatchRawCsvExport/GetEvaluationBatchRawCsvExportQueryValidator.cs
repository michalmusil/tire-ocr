using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Queries.GetEvaluationBatchRawCsvExport;

public class GetEvaluationBatchRawCsvExportQueryValidator : AbstractValidator<GetEvaluationBatchRawCsvExportQuery>
{
    public GetEvaluationBatchRawCsvExportQueryValidator()
    {
        RuleFor(x => x.Id.ToString())
            .IsGuid();
    }
}