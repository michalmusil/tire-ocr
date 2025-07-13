using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Common.DataAccess.Configuration.NodeTypeAggregate;

public class NodeTypeConfiguration : IEntityTypeConfiguration<Domain.NodeTypeAggregate.NodeType>
{
    public void Configure(EntityTypeBuilder<Domain.NodeTypeAggregate.NodeType> builder)
    {
        builder.HasKey(nt => nt.Id);
        builder.Property(nt => nt.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Ignore(nt => nt.AvailableProcedures);

        builder.HasMany(nt => nt._availableProcedures)
            .WithOne()
            .HasForeignKey(np => np.NodeTypeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}