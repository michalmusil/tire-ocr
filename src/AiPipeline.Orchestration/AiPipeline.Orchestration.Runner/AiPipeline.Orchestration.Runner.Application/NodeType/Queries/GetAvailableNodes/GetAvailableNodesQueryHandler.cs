using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.NodeType.Dtos;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.NodeType.Queries.GetAvailableNodes;

public class GetAvailableNodesQueryHandler : IQueryHandler<GetAvailableNodesQuery, PaginatedCollection<GetNodeDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetAvailableNodesQueryHandler> _logger;

    public GetAvailableNodesQueryHandler(IUnitOfWork unitOfWork,
        ILogger<GetAvailableNodesQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<DataResult<PaginatedCollection<GetNodeDto>>> Handle(
        GetAvailableNodesQuery request,
        CancellationToken cancellationToken
    )
    {
        var foundNodes = await _unitOfWork.NodeTypeEntityRepository.GetNodeTypesPaginatedAsync(request.Pagination);
        var nodeDtos = foundNodes.Items
            .Select(GetNodeDto.FromDomain)
            .ToList();
        var dtosCollection = new PaginatedCollection<GetNodeDto>(
            nodeDtos,
            totalCount: foundNodes.Pagination.TotalCount,
            pageNumber: foundNodes.Pagination.PageNumber,
            pageSize: foundNodes.Pagination.PageSize
        );

        return DataResult<PaginatedCollection<GetNodeDto>>.Success(dtosCollection);
    }
}