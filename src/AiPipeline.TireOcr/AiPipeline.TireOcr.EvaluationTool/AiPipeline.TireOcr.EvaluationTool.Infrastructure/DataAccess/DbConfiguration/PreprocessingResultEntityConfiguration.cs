using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.DataAccess.DbConfiguration;

public class PreprocessingResultEntityConfiguration : IEntityTypeConfiguration<PreprocessingResultEntity>
{
    public void Configure(EntityTypeBuilder<PreprocessingResultEntity> builder)
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

        builder.OwnsOne(pr => pr.PreprocessingResult);
    }
}