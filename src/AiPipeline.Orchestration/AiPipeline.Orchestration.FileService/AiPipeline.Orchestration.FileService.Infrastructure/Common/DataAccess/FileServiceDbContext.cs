using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace AiPipeline.Orchestration.FileService.Infrastructure.Common.DataAccess;

public class FileServiceDbContext : DbContext
{
    public FileServiceDbContext(DbContextOptions<FileServiceDbContext> options)
        : base(options)
    {
    }

    public DbSet<Domain.FileAggregate.File> Files => Set<Domain.FileAggregate.File>();

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