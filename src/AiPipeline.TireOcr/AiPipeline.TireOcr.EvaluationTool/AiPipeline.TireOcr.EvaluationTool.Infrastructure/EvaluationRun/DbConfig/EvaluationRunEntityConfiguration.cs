using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.Evaluation;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.DbConfig;

public class EvaluationRunEntityConfiguration : IEntityTypeConfiguration<EvaluationRunEntity>
{
    public void Configure(EntityTypeBuilder<EvaluationRunEntity> builder)
    {
        builder.HasKey(er => er.Id);

        builder.Property(er => er.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(er => er.Title)
            .IsRequired();

        builder.Property(er => er.StartedAt)
            .IsRequired();

        builder.Property(er => er.FinishedAt)
            .IsRequired(false);

        builder.Property(er => er.PreprocessingType)
            .IsRequired();

        builder.Property(er => er.OcrType)
            .IsRequired();

        builder.Property(er => er.PostprocessingType)
            .IsRequired();

        builder.Property(er => er.DbMatchingType)
            .IsRequired();

        builder.Property(er => er.CreatedAt)
            .IsRequired();

        builder.Property(er => er.UpdatedAt)
            .IsRequired();

        builder.OwnsOne(er => er.InputImage);

        builder.OwnsOne(er => er.RunFailure);


        builder.Ignore(er => er.HasFinished);
        builder.Ignore(er => er.HasRunFailed);
        builder.Ignore(er => er.TotalExecutionDuration);


        builder.HasOne<EvaluationRunBatchEntity>()
            .WithMany(erb => erb._evaluationRuns)
            .HasForeignKey(er => er.BatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(er => er.Evaluation)
            .WithOne()
            .HasForeignKey<EvaluationEntity>(er => er.RunId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(er => er.PreprocessingResult)
            .WithOne()
            .HasForeignKey<PreprocessingResultEntity>(pr => pr.RunId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(er => er.OcrResult)
            .WithOne()
            .HasForeignKey<OcrResultEntity>(or => or.RunId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(er => er.PostprocessingResult)
            .WithOne()
            .HasForeignKey<PostprocessingResultEntity>(pr => pr.RunId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(er => er.DbMatchingResult)
            .WithOne()
            .HasForeignKey<DbMatchingResultEntity>(dmr => dmr.RunId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}