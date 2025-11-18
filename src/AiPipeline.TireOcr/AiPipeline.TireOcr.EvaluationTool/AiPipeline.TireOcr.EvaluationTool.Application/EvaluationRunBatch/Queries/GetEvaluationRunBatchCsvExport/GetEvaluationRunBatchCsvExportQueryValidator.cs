using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Queries.GetEvaluationRunBatchCsvExport;

public class GetEvaluationRunBatchCsvExportQueryValidator : AbstractValidator<GetEvaluationRunBatchCsvExportQuery>
{
    public GetEvaluationRunBatchCsvExportQueryValidator()
    {
        RuleFor(x => x.Id.ToString())
            .IsGuid();
    }
}