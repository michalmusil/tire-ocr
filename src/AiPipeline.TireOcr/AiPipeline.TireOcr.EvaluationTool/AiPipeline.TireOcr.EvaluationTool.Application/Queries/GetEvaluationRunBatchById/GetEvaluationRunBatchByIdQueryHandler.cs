using AiPipeline.TireOcr.EvaluationTool.Application.DataAccess;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Queries.GetEvaluationRunBatchById;

public class
    GetEvaluationRunBatchByIdQueryHandler : IQueryHandler<GetEvaluationRunBatchByIdQuery, EvaluationRunBatchFullDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetEvaluationRunBatchByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DataResult<EvaluationRunBatchFullDto>> Handle(GetEvaluationRunBatchByIdQuery request,
        CancellationToken cancellationToken)
    {
        var batch = await _unitOfWork.EvaluationRunBatchRepository.GetEvaluationRunBatchByIdAsync(
            id: request.Id,
            includeFullData: true
        );
        if (batch is null)
            return DataResult<EvaluationRunBatchFullDto>.NotFound($"Batch with id '{request.Id}' not found");

        var dto = EvaluationRunBatchFullDto.FromDomain(batch);
        return DataResult<EvaluationRunBatchFullDto>.Success(dto);
    }
}