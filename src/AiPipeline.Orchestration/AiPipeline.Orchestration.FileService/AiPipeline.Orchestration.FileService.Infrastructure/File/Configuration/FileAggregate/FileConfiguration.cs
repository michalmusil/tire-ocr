using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiPipeline.Orchestration.FileService.Infrastructure.File.Configuration.FileAggregate;

public class FileConfiguration : IEntityTypeConfiguration<Domain.FileAggregate.File>
{
    public void Configure(EntityTypeBuilder<Domain.FileAggregate.File> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(f => f.UserId)
            .IsRequired();

        builder.Property(f => f.FileStorageScope)
            .IsRequired();

        builder.Property(f => f.StorageProvider)
            .IsRequired();

        builder.Property(f => f.Path)
            .IsRequired();

        builder.Property(f => f.ContentType)
            .IsRequired();

        builder.Property(nt => nt.CreatedAt)
            .IsRequired();

        builder.Property(nt => nt.UpdatedAt)
            .IsRequired();
    }
}