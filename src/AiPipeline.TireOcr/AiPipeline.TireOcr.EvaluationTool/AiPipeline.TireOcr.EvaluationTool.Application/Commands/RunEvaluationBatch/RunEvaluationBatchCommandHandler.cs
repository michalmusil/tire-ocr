using AiPipeline.TireOcr.EvaluationTool.Application.DataAccess;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;
using AiPipeline.TireOcr.EvaluationTool.Application.Facades;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Commands.RunEvaluationBatch;

public class RunEvaluationBatchCommandHandler : ICommandHandler<RunEvaluationBatchCommand, EvaluationRunBatchDto>
{
    private readonly IRunFacade _runFacade;
    private readonly IUnitOfWork _unitOfWork;

    public RunEvaluationBatchCommandHandler(IRunFacade runFacade, IUnitOfWork unitOfWork)
    {
        _runFacade = runFacade;
        _unitOfWork = unitOfWork;
    }

    public async Task<DataResult<EvaluationRunBatchDto>> Handle(RunEvaluationBatchCommand request,
        CancellationToken cancellationToken)
    {
        var inputDetails = new RunEntityInputDetailsDto(
            Id: request.BatchId,
            Title: request.BatchTitle
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
            return DataResult<EvaluationRunBatchDto>.Failure(result.Failures);

        var batch = result.Data!;
        await _unitOfWork.EvaluationRunBatchRepository.Add(batch);
        await _unitOfWork.SaveChangesAsync();

        var dto = EvaluationRunBatchDto.FromDomain(result.Data!);
        return DataResult<EvaluationRunBatchDto>.Success(dto);
    }
}