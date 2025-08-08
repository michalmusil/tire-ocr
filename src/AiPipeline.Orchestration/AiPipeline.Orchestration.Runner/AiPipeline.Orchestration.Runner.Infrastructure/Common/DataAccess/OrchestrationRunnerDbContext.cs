using System.Reflection;
using AiPipeline.Orchestration.Runner.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Common.DataAccess;

public class OrchestrationRunnerDbContext : DbContext
{
    public OrchestrationRunnerDbContext(DbContextOptions<OrchestrationRunnerDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Domain.NodeTypeAggregate.NodeType> NodeTypes => Set<Domain.NodeTypeAggregate.NodeType>();

    public DbSet<Domain.PipelineResultAggregate.PipelineResult> PipelineResults =>
        Set<Domain.PipelineResultAggregate.PipelineResult>();

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