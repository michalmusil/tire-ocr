using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Repositories;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Repositories;
using AiPipeline.TireOcr.EvaluationTool.Application.User.Repositories;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Common.DataAccess;

public interface IUnitOfWork
{
    public IEvaluationRunEntityRepository EvaluationRunRepository { get; }
    public IEvaluationRunBatchEntityRepository EvaluationRunBatchRepository { get; }
    public IUserEntityRepository UserRepository { get; }

    public Task SaveChangesAsync();
}