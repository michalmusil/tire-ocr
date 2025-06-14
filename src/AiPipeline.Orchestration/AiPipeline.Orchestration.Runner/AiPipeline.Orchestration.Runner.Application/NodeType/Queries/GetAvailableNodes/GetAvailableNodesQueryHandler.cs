using AiPipeline.Orchestration.Runner.Application.NodeType.Dtos;
using AiPipeline.Orchestration.Runner.Application.NodeType.Repositories;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.NodeType.Queries.GetAvailableNodes;

public class GetAvailableNodesQueryHandler : IQueryHandler<GetAvailableNodesQuery, PaginatedCollection<NodeDto>>
{
    private readonly INodeTypeRepository _nodeTypeRepository;
    private readonly ILogger<GetAvailableNodesQueryHandler> _logger;

    public GetAvailableNodesQueryHandler(INodeTypeRepository nodeTypeRepository,
        ILogger<GetAvailableNodesQueryHandler> logger)
    {
        _nodeTypeRepository = nodeTypeRepository;
        _logger = logger;
    }

    public async Task<DataResult<PaginatedCollection<NodeDto>>> Handle(
        GetAvailableNodesQuery request,
        CancellationToken cancellationToken
    )
    {
        var foundNodes = await _nodeTypeRepository.GetNodeTypesPaginatedAsync(request.Pagination);
        var nodeDtos = foundNodes.Items
            .Select(NodeDto.FromDomain)
            .ToList();
        var dtosCollection = new PaginatedCollection<NodeDto>(
            nodeDtos,
            totalCount: foundNodes.Pagination.TotalCount,
            pageNumber: foundNodes.Pagination.PageNumber,
            pageSize: foundNodes.Pagination.PageSize
        );

        return DataResult<PaginatedCollection<NodeDto>>.Success(dtosCollection);
    }
}