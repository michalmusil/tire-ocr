using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.DbConfig;

public class OcrResultEntityConfiguration : IEntityTypeConfiguration<OcrResultEntity>
{
    public void Configure(EntityTypeBuilder<OcrResultEntity> builder)
    {
        builder.HasKey(or => or.Id);

        builder.Property(or => or.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(e => e.RunId)
            .IsRequired();

        builder.Property(or => or.DurationMs)
            .IsRequired();

        builder.Property(or => or.DetectedCode)
            .IsRequired();

        builder.Property(or => or.DetectedManufacturer)
            .IsRequired(false);

        builder.Property(or => or.InputUnitCount)
            .IsRequired(false);

        builder.Property(or => or.OutputUnitCount)
            .IsRequired(false);

        builder.Property(or => or.BillingUnit)
            .IsRequired(false);

        builder.Property(or => or.EstimatedCost)
            .IsRequired(false);

        builder.Property(or => or.EstimatedCostCurrency)
            .IsRequired(false);

        builder.Property(or => or.CreatedAt)
            .IsRequired();

        builder.Property(or => or.UpdatedAt)
            .IsRequired();
    }
}