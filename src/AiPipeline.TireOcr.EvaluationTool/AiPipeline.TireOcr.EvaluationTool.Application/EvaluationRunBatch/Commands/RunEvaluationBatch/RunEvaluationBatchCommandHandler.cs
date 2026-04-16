using AiPipeline.TireOcr.EvaluationTool.Application.Common.DataAccess;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Facades;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Services;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Commands.RunEvaluationBatch;

public class RunEvaluationBatchCommandHandler : ICommandHandler<RunEvaluationBatchCommand, EvaluationRunBatchFullDto>
{
    private readonly IRunFacade _runFacade;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBatchEvaluationService _batchEvaluationService;

    public RunEvaluationBatchCommandHandler(IRunFacade runFacade, IUnitOfWork unitOfWork,
        IBatchEvaluationService batchEvaluationService)
    {
        _runFacade = runFacade;
        _unitOfWork = unitOfWork;
        _batchEvaluationService = batchEvaluationService;
    }

    public async Task<DataResult<EvaluationRunBatchFullDto>> Handle(RunEvaluationBatchCommand request,
        CancellationToken cancellationToken)
    {
        var inputDetails = new RunEntityInputDetailsDto(
            Id: request.BatchId,
            Title: request.BatchTitle,
            Description: request.BatchDescription
        );

        var imageUrlsWithExpectedTireCodes = request.ImageUrlsWithExpectedTireCodeLabels
            .ToDictionary(
                keySelector: kvp => kvp.Key,
                elementSelector: kvp =>
                {
                    var expectedTireCodeResult = kvp.Value is null
                        ? null
                        : TireCodeValueObject.FromLabelString(kvp.Value);
                    return expectedTireCodeResult?.Data;
                }
            );

        var result = await _runFacade.RunEvaluationBatchAsync(
            imageUrls: imageUrlsWithExpectedTireCodes,
            batchSize: request.ProcessingBatchSize,
            runConfig: request.RunConfig,
            runEntityInputDetailsDto: inputDetails
        );

        if (result.IsFailure)
            return DataResult<EvaluationRunBatchFullDto>.Failure(result.Failures);

        var batch = result.Data!;
        await _unitOfWork.EvaluationRunBatchRepository.Add(batch);
        await _unitOfWork.SaveChangesAsync();

        var batchEvaluationResult = await _batchEvaluationService.EvaluateBatch(batch);

        var dto = EvaluationRunBatchFullDto.FromDomain(result.Data!, batchEvaluationResult.Data);
        return DataResult<EvaluationRunBatchFullDto>.Success(dto);
    }
}