using AiPipeline.TireOcr.EvaluationTool.Application.DataAccess;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Queries.GetEvaluationRunBatchesPaginated;

public class GetEvaluationRunBatchesPaginatedQueryHandler
    : IQueryHandler<GetEvaluationRunBatchesPaginatedQuery, PaginatedCollection<EvaluationRunBatchLightDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetEvaluationRunBatchesPaginatedQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DataResult<PaginatedCollection<EvaluationRunBatchLightDto>>> Handle(
        GetEvaluationRunBatchesPaginatedQuery request,
        CancellationToken cancellationToken
    )
    {
        var foundResults = await _unitOfWork
            .EvaluationRunBatchRepository
            .GetEvaluationRunBatchesPaginatedAsync(request.Pagination);

        var resultDtos = foundResults.Items
            .ToList();

        var dtosCollection = new PaginatedCollection<EvaluationRunBatchLightDto>(
            resultDtos,
            totalCount: foundResults.Pagination.TotalCount,
            pageNumber: foundResults.Pagination.PageNumber,
            pageSize: foundResults.Pagination.PageSize
        );

        return DataResult<PaginatedCollection<EvaluationRunBatchLightDto>>.Success(dtosCollection);
    }
}