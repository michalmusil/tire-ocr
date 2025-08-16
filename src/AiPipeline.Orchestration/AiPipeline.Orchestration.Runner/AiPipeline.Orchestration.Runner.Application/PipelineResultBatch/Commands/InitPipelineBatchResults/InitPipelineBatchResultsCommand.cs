using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos;
using AiPipeline.Orchestration.Runner.Domain.PipelineAggregate;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResultBatch.Commands.InitPipelineBatchResults;

public record InitPipelineBatchResultsCommand(PipelineBatch Batch): ICommand<GetPipelineBatchDto>;