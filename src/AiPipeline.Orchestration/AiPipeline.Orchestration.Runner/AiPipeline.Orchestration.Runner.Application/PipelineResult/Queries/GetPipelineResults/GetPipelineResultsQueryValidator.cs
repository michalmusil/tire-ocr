using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Queries.GetPipelineResults;

public class GetPipelineResultsQueryValidator : AbstractValidator<GetPipelineResultsQuery>
{
    public GetPipelineResultsQueryValidator()
    {
        RuleFor(q => q.Pagination)
            .IsValidPagination();
    }
}