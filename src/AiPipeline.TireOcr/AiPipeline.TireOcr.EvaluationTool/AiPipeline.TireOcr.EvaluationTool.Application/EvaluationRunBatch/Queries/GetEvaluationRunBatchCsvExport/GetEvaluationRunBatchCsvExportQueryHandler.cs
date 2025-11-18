using AiPipeline.TireOcr.EvaluationTool.Application.Common.DataAccess;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Services;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Queries.GetEvaluationRunBatchCsvExport;

public class GetEvaluationRunBatchCsvExportQueryHandler : IQueryHandler<GetEvaluationRunBatchCsvExportQuery, byte[]>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBatchCsvExportService _batchCsvExportService;

    public GetEvaluationRunBatchCsvExportQueryHandler(IUnitOfWork unitOfWork,
        IBatchCsvExportService batchCsvExportService)
    {
        _unitOfWork = unitOfWork;
        _batchCsvExportService = batchCsvExportService;
    }

    public async Task<DataResult<byte[]>> Handle(GetEvaluationRunBatchCsvExportQuery request,
        CancellationToken cancellationToken)
    {
        var batch = await _unitOfWork.EvaluationRunBatchRepository.GetEvaluationRunBatchByIdAsync(id: request.Id);
        if (batch is null)
            return DataResult<byte[]>.NotFound($"Batch with id '{request.Id}' not found");

        var runs = await _unitOfWork.EvaluationRunRepository.GetEvaluationRunsByBatchIdAsync(batch.Id);

        var batchWithRuns = new EvaluationRunBatchEntity(
            id: batch.Id,
            title: batch.Title,
            description: batch.Description,
            evaluationRuns: runs.ToList()
        );

        var csvContent = await _batchCsvExportService.ExportBatch(batchWithRuns);
        return csvContent.Map(
            onFailure: DataResult<byte[]>.Failure,
            onSuccess: DataResult<byte[]>.Success
        );
    }
}