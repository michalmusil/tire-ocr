using AiPipeline.TireOcr.EvaluationTool.Application.Repositories;

namespace AiPipeline.TireOcr.EvaluationTool.Application.DataAccess;

public interface IUnitOfWork
{
    public IEvaluationRunEntityRepository EvaluationRunRepository { get; }
    public IEvaluationRunBatchEntityRepository EvaluationRunBatchRepository { get; }

    public Task SaveChangesAsync();
}