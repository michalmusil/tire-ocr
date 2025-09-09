using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiPipeline.Orchestration.Runner.Infrastructure.PipelineResultBatch.DbConfiguration;

public class
    PipelineResultBatchConfiguration : IEntityTypeConfiguration<Domain.PipelineResultBatchAggregate.PipelineResultBatch>
{
    public void Configure(EntityTypeBuilder<Domain.PipelineResultBatchAggregate.PipelineResultBatch> builder)
    {
        builder.HasKey(pr => pr.Id);
        builder.Property(pr => pr.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(pr => pr.UserId)
            .IsRequired();

        builder.Property(pr => pr.FinishedAt)
            .IsRequired(false);

        builder.Property(nt => nt.CreatedAt)
            .IsRequired();

        builder.Property(nt => nt.UpdatedAt)
            .IsRequired();

        builder.HasMany<Domain.PipelineResultAggregate.PipelineResult>()
            .WithOne()
            .HasForeignKey(pr => pr.BatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Domain.UserAggregate.User>()
            .WithMany()
            .HasForeignKey(prb => prb.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}