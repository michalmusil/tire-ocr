using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.Evaluation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.DbConfig;

public class EvaluationEntityConfiguration : IEntityTypeConfiguration<EvaluationEntity>
{
    public void Configure(EntityTypeBuilder<EvaluationEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(e => e.RunId)
            .IsRequired();

        builder.Property(e => e.TotalDistance)
            .IsRequired();

        builder.Property(e => e.FullMatchParameterCount)
            .IsRequired();

        builder.Property(e => e.EstimatedAccuracy)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.OwnsOne(e => e.ExpectedTireCode);

        builder.OwnsOne(e => e.VehicleClassEvaluation);
        builder.OwnsOne(e => e.WidthEvaluation);
        builder.OwnsOne(e => e.DiameterEvaluation);
        builder.OwnsOne(e => e.AspectRatioEvaluation);
        builder.OwnsOne(e => e.ConstructionEvaluation);
        builder.OwnsOne(e => e.LoadRangeEvaluation);
        builder.OwnsOne(e => e.LoadIndexEvaluation);
        builder.OwnsOne(e => e.LoadIndex2Evaluation);
        builder.OwnsOne(e => e.SpeedRatingEvaluation);
        
    }
}