using AiPipeline.Orchestration.Runner.Domain.PipelineResultAggregate;
using AiPipeline.Orchestration.Runner.Infrastructure.Common.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Common.DataAccess.Configuration.PipelineResultAggregate;

public class PipelineStepResultConfiguration : IEntityTypeConfiguration<PipelineStepResult>
{
    public void Configure(EntityTypeBuilder<PipelineStepResult> builder)
    {
        builder.HasKey(pr => pr.Id);
        builder.Property(pr => pr.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(pr => pr.ResultId)
            .IsRequired();

        builder.Property(pr => pr.NodeId)
            .IsRequired();

        builder.Property(pr => pr.NodeProcedureId)
            .IsRequired();

        builder.Property(pr => pr.FinishedAt)
            .IsRequired();

        builder.Property(pr => pr.WasSuccessful)
            .IsRequired();

        builder.Property(pr => pr.Result)
            .HasConversion(JsonUtils.GetApElementJsonValueConverter()!);

        builder.Property(pr => pr.FailureReason)
            .HasConversion(JsonUtils.GetDefaultJsonValueConverter<PipelineFailureReason?>());


        builder.HasOne<Domain.PipelineResultAggregate.PipelineResult>()
            .WithMany(pr => pr._stepResults)
            .HasForeignKey(psr => psr.ResultId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}