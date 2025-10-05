using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.DataAccess.DbConfiguration;

public class EvaluationRunBatchBatchEntityConfiguration : IEntityTypeConfiguration<EvaluationRunBatchEntity>
{
    public void Configure(EntityTypeBuilder<EvaluationRunBatchEntity> builder)
    {
        builder.HasKey(erb => erb.Id);

        builder.Property(erb => erb.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(erb => erb.Title)
            .IsRequired();

        builder.Property(erb => erb.CreatedAt)
            .IsRequired();

        builder.Property(erb => erb.UpdatedAt)
            .IsRequired();

        builder.Ignore(erb => erb.StartedAt);

        builder.Ignore(erb => erb.FinishedAt);

        builder.Ignore(erb => erb.EvaluationRuns);


        builder.HasMany<EvaluationRunEntity>(erb => erb._evaluationRuns)
            .WithOne()
            .HasForeignKey(er => er.BatchId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}