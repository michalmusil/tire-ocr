using AiPipeline.Orchestration.Runner.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiPipeline.Orchestration.Runner.Infrastructure.User.DbConfiguration;

public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.HasKey(ak => ak.Key);
        builder.Property(ak => ak.Key)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(ak => ak.UserId)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(ak => ak.Name)
            .IsRequired();
        builder.Property(ak => ak.ValidUntil)
            .IsRequired(false);
        builder.Property(ak => ak.CreatedAt)
            .IsRequired();
        builder.Property(ak => ak.UpdatedAt)
            .IsRequired();

        builder.HasOne<Domain.UserAggregate.User>()
            .WithMany(u => u._apiKeys)
            .HasForeignKey(ak => ak.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}