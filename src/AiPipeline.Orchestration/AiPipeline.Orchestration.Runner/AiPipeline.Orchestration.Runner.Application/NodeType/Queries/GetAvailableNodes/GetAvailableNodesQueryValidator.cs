using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.Orchestration.Runner.Application.NodeType.Queries.GetAvailableNodes;

public class GetAvailableNodesQueryValidator : AbstractValidator<GetAvailableNodesQuery>
{
    public GetAvailableNodesQueryValidator()
    {
        RuleFor(q => q.Pagination)
            .IsValidPagination();
    }
}