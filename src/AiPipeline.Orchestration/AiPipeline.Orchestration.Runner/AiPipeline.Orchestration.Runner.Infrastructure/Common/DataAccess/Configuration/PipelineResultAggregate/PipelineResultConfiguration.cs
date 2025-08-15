using AiPipeline.Orchestration.Runner.Infrastructure.Common.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Common.DataAccess.Configuration.PipelineResultAggregate;

public class PipelineResultConfiguration : IEntityTypeConfiguration<Domain.PipelineResultAggregate.PipelineResult>
{
    public void Configure(EntityTypeBuilder<Domain.PipelineResultAggregate.PipelineResult> builder)
    {
        builder.HasKey(pr => pr.Id);
        builder.Property(pr => pr.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(pr => pr.PipelineId)
            .IsRequired();

        builder.Property(pr => pr.UserId)
            .IsRequired();

        builder.Property(pr => pr.InitialInput)
            .IsRequired(false)
            .HasConversion(JsonUtils.GetApElementJsonValueConverter()!);

        builder.Property(pr => pr.FinishedAt)
            .IsRequired(false);

        builder.Property(nt => nt.CreatedAt)
            .IsRequired();

        builder.Property(nt => nt.UpdatedAt)
            .IsRequired();

        builder.Ignore(pr => pr.StepResults);

        builder.HasMany(pr => pr._stepResults)
            .WithOne()
            .HasForeignKey(psr => psr.ResultId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Domain.UserAggregate.User>()
            .WithMany()
            .HasForeignKey(pr => pr.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}