using AiPipeline.TireOcr.EvaluationTool.Application.Common.DataAccess;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Queries.GetEvaluationRunsPaginated;

public class GetEvaluationRunsPaginatedQueryHandler
    : IQueryHandler<GetEvaluationRunsPaginatedQuery, PaginatedCollection<EvaluationRunDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetEvaluationRunsPaginatedQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DataResult<PaginatedCollection<EvaluationRunDto>>> Handle(
        GetEvaluationRunsPaginatedQuery request,
        CancellationToken cancellationToken
    )
    {
        var foundResults = await _unitOfWork
            .EvaluationRunRepository
            .GetEvaluationRunsPaginatedAsync(request.Pagination);

        var resultDtos = foundResults.Items
            .Select(EvaluationRunDto.FromDomain)
            .ToList();

        var dtosCollection = new PaginatedCollection<EvaluationRunDto>(
            resultDtos,
            totalCount: foundResults.Pagination.TotalCount,
            pageNumber: foundResults.Pagination.PageNumber,
            pageSize: foundResults.Pagination.PageSize
        );

        return DataResult<PaginatedCollection<EvaluationRunDto>>.Success(dtosCollection);
    }
}