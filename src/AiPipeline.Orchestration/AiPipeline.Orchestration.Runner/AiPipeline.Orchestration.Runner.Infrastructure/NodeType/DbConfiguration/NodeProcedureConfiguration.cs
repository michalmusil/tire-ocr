using AiPipeline.Orchestration.Runner.Infrastructure.Common.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiPipeline.Orchestration.Runner.Infrastructure.NodeType.DbConfiguration;

public class NodeProcedureConfiguration : IEntityTypeConfiguration<Domain.NodeTypeAggregate.NodeProcedure>
{
    public void Configure(EntityTypeBuilder<Domain.NodeTypeAggregate.NodeProcedure> builder)
    {
        builder.HasKey(np => new { np.Id, np.NodeTypeId });

        builder.Property(np => np.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(np => np.NodeTypeId)
            .IsRequired();

        builder.Property(np => np.SchemaVersion)
            .IsRequired();

        builder.Property(np => np.InputSchema)
            .IsRequired()
            .HasConversion(JsonUtils.GetApElementJsonValueConverter());

        builder.Property(np => np.OutputSchema)
            .IsRequired()
            .HasConversion(JsonUtils.GetApElementJsonValueConverter());

        builder.Property(nt => nt.CreatedAt)
            .IsRequired();

        builder.Property(nt => nt.UpdatedAt)
            .IsRequired();

        builder.HasOne<Domain.NodeTypeAggregate.NodeType>()
            .WithMany(nt => nt._availableProcedures)
            .HasForeignKey(np => np.NodeTypeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}