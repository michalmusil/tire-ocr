using System.Reflection;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.Evaluation;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;
using Microsoft.EntityFrameworkCore;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Common.DataAccess;

public class EvaluationToolDbContext : DbContext
{
    public EvaluationToolDbContext(DbContextOptions<EvaluationToolDbContext> options)
        : base(options)
    {
    }

    public DbSet<EvaluationRunEntity> EvaluationRuns => Set<EvaluationRunEntity>();
    public DbSet<EvaluationEntity> Evaluations => Set<EvaluationEntity>();
    public DbSet<PreprocessingResultEntity> PreprocessingResults => Set<PreprocessingResultEntity>();
    public DbSet<OcrResultEntity> OcrResults => Set<OcrResultEntity>();
    public DbSet<PostprocessingResultEntity> PostprocessingResults => Set<PostprocessingResultEntity>();
    public DbSet<DbMatchingResultEntity> DbMatchingResults => Set<DbMatchingResultEntity>();

    public DbSet<EvaluationRunBatchEntity> EvaluationRunBatches => Set<EvaluationRunBatchEntity>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return result;
    }

    public override int SaveChanges()
    {
        return SaveChangesAsync().GetAwaiter().GetResult();
    }
}