using AiPipeline.TireOcr.EvaluationTool.Application.DataAccess;
using AiPipeline.TireOcr.EvaluationTool.Application.Repositories;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.DataAccess;

public class UnitOfWork : IUnitOfWork
{
    private readonly EvaluationToolDbContext _dbContext;
    public IEvaluationRunEntityRepository EvaluationRunRepository { get; }
    public IEvaluationRunBatchEntityRepository EvaluationRunBatchRepository { get; }

    public UnitOfWork(EvaluationToolDbContext dbContext, IEvaluationRunEntityRepository evaluationRunRepository,
        IEvaluationRunBatchEntityRepository evaluationRunBatchRepository)
    {
        _dbContext = dbContext;
        EvaluationRunRepository = evaluationRunRepository;
        EvaluationRunBatchRepository = evaluationRunBatchRepository;
    }


    public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
}