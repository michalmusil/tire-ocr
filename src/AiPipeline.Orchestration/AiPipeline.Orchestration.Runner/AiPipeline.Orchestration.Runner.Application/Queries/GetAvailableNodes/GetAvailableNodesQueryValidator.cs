using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.Orchestration.Runner.Application.Queries.GetAvailableNodes;

public class GetAvailableNodesQueryValidator : AbstractValidator<GetAvailableNodesQuery>
{
    public GetAvailableNodesQueryValidator()
    {
        RuleFor(q => q.Pagination)
            .IsValidPagination();
    }
}