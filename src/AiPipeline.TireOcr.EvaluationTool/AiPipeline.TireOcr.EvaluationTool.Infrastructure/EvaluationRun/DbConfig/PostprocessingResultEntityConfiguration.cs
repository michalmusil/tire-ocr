using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.DbConfig;

public class PostprocessingResultEntityConfiguration : IEntityTypeConfiguration<PostprocessingResultEntity>
{
    public void Configure(EntityTypeBuilder<PostprocessingResultEntity> builder)
    {
        builder.HasKey(pr => pr.Id);

        builder.Property(pr => pr.Id)
            .IsRequired()
            .ValueGeneratedNever();
        
        builder.Property(e => e.RunId)
            .IsRequired();

        builder.Property(pr => pr.DurationMs)
            .IsRequired();

        builder.Property(pr => pr.CreatedAt)
            .IsRequired();

        builder.Property(pr => pr.UpdatedAt)
            .IsRequired();

        builder.OwnsOne(pr => pr.TireCode);
    }
}