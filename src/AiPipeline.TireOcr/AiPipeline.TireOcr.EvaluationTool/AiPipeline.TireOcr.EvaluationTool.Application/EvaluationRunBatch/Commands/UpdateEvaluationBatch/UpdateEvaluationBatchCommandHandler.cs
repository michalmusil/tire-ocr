using AiPipeline.TireOcr.EvaluationTool.Application.Common.DataAccess;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Services;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Commands.UpdateEvaluationBatch;

public class
    UpdateEvaluationBatchCommandHandler : ICommandHandler<UpdateEvaluationBatchCommand, EvaluationRunBatchFullDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBatchEvaluationService _batchEvaluationService;

    public UpdateEvaluationBatchCommandHandler(IUnitOfWork unitOfWork, IBatchEvaluationService batchEvaluationService)
    {
        _unitOfWork = unitOfWork;
        _batchEvaluationService = batchEvaluationService;
    }

    public async Task<DataResult<EvaluationRunBatchFullDto>> Handle(UpdateEvaluationBatchCommand request,
        CancellationToken cancellationToken)
    {
        var foundBatch = await _unitOfWork.EvaluationRunBatchRepository.GetEvaluationRunBatchByIdAsync(request.BatchId);
        if (foundBatch is null)
            return DataResult<EvaluationRunBatchFullDto>.NotFound($"Batch with id {request.BatchId} does not exist.");
        

        var updateResults = new List<Result>();

        // Performing actual updates
        if (request.BatchTitle is not null)
            updateResults.Add(foundBatch.SetTitle(request.BatchTitle));

        var failures = updateResults
            .SelectMany(r => r.Failures)
            .ToArray();

        if (failures.Any())
            return DataResult<EvaluationRunBatchFullDto>.Failure(failures.ToArray());
        
        // Getting child runs
        var runs = await _unitOfWork.EvaluationRunRepository.GetEvaluationRunsByBatchIdAsync(foundBatch.Id);
        var batchWithRuns = new EvaluationRunBatchEntity(
            id: foundBatch.Id,
            title: foundBatch.Title,
            evaluationRuns: runs.ToList()
        );

        var batchEvaluationResult = await _batchEvaluationService.EvaluateBatch(batchWithRuns);
        if (batchEvaluationResult.IsFailure)
            return DataResult<EvaluationRunBatchFullDto>.Failure(batchEvaluationResult.Failures);

        await _unitOfWork.SaveChangesAsync();
        var dto = EvaluationRunBatchFullDto.FromDomain(batchWithRuns, batchEvaluationResult.Data!);

        return DataResult<EvaluationRunBatchFullDto>.Success(dto);
    }
}