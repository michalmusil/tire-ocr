using AiPipeline.TireOcr.EvaluationTool.Application.Common.DataAccess;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Repositories;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Repositories;
using AiPipeline.TireOcr.EvaluationTool.Application.User.Repositories;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Common.DataAccess;

public class UnitOfWork : IUnitOfWork
{
    private readonly EvaluationToolDbContext _dbContext;
    public IEvaluationRunEntityRepository EvaluationRunRepository { get; }
    public IEvaluationRunBatchEntityRepository EvaluationRunBatchRepository { get; }
    public IUserEntityRepository UserRepository { get; }

    public UnitOfWork(EvaluationToolDbContext dbContext, IEvaluationRunEntityRepository evaluationRunRepository,
        IEvaluationRunBatchEntityRepository evaluationRunBatchRepository, IUserEntityRepository userRepository)
    {
        _dbContext = dbContext;
        EvaluationRunRepository = evaluationRunRepository;
        EvaluationRunBatchRepository = evaluationRunBatchRepository;
        UserRepository = userRepository;
    }


    public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
}