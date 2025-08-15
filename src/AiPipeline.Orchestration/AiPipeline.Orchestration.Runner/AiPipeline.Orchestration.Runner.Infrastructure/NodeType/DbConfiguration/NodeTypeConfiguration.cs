using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiPipeline.Orchestration.Runner.Infrastructure.NodeType.DbConfiguration;

public class NodeTypeConfiguration : IEntityTypeConfiguration<Domain.NodeTypeAggregate.NodeType>
{
    public void Configure(EntityTypeBuilder<Domain.NodeTypeAggregate.NodeType> builder)
    {
        builder.HasKey(nt => nt.Id);
        builder.Property(nt => nt.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(nt => nt.CreatedAt)
            .IsRequired();
        
        builder.Property(nt => nt.UpdatedAt)
            .IsRequired();

        builder.Ignore(nt => nt.AvailableProcedures);

        builder.HasMany(nt => nt._availableProcedures)
            .WithOne()
            .HasForeignKey(np => np.NodeTypeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}