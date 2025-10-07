using AiPipeline.TireOcr.EvaluationTool.Application.DataAccess;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.Services;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Queries.GetEvaluationRunBatchById;

public class
    GetEvaluationRunBatchByIdQueryHandler : IQueryHandler<GetEvaluationRunBatchByIdQuery, EvaluationRunBatchFullDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBatchEvaluationService _batchEvaluationService;

    public GetEvaluationRunBatchByIdQueryHandler(IUnitOfWork unitOfWork, IBatchEvaluationService batchEvaluationService)
    {
        _unitOfWork = unitOfWork;
        _batchEvaluationService = batchEvaluationService;
    }

    public async Task<DataResult<EvaluationRunBatchFullDto>> Handle(GetEvaluationRunBatchByIdQuery request,
        CancellationToken cancellationToken)
    {
        var batch = await _unitOfWork.EvaluationRunBatchRepository.GetEvaluationRunBatchByIdAsync(id: request.Id);
        if (batch is null)
            return DataResult<EvaluationRunBatchFullDto>.NotFound($"Batch with id '{request.Id}' not found");

        var runs = await _unitOfWork.EvaluationRunRepository.GetEvaluationRunsByBatchIdAsync(batch.Id);

        var batchWithRuns = new EvaluationRunBatchEntity(
            id: batch.Id,
            title: batch.Title,
            evaluationRuns: runs.ToList()
        );

        var batchEvaluation = await _batchEvaluationService.EvaluateBatch(batchWithRuns);

        var dto = EvaluationRunBatchFullDto.FromDomain(batchWithRuns, batchEvaluation.Data);
        return DataResult<EvaluationRunBatchFullDto>.Success(dto);
    }
}