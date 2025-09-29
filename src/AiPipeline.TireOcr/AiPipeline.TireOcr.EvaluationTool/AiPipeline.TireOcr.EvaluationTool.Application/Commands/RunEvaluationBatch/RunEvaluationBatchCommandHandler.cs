using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;
using AiPipeline.TireOcr.EvaluationTool.Application.Facades;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Commands.RunEvaluationBatch;

public class RunEvaluationBatchCommandHandler : ICommandHandler<RunEvaluationBatchCommand, EvaluationRunBatchDto>
{
    private readonly IRunFacade _runFacade;

    public RunEvaluationBatchCommandHandler(IRunFacade runFacade)
    {
        _runFacade = runFacade;
    }

    public async Task<DataResult<EvaluationRunBatchDto>> Handle(RunEvaluationBatchCommand request,
        CancellationToken cancellationToken)
    {
        var inputDetails = new RunEntityInputDetailsDto(
            Id: request.BatchId,
            Title: request.BatchTitle
        );

        var result = await _runFacade.RunEvaluationBatchAsync(
            imageUrls: request.InputImagesWithExpectedTireCodes
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToDomain()),
            batchSize: request.ProcessingBatchSize,
            runConfig: request.RunConfig,
            runEntityInputDetailsDto: inputDetails
        );

        if (result.IsFailure)
            return DataResult<EvaluationRunBatchDto>.Failure(result.Failures);

        var dto = EvaluationRunBatchDto.FromDomain(result.Data!);
        return DataResult<EvaluationRunBatchDto>.Success(dto);
    }
}