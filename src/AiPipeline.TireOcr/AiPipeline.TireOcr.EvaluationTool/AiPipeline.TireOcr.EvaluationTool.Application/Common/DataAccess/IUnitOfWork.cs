using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Repositories;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Repositories;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Common.DataAccess;

public interface IUnitOfWork
{
    public IEvaluationRunEntityRepository EvaluationRunRepository { get; }
    public IEvaluationRunBatchEntityRepository EvaluationRunBatchRepository { get; }

    public Task SaveChangesAsync();
}