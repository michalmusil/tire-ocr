using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiPipeline.Orchestration.Runner.Infrastructure.User.DbConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<Domain.UserAggregate.User>
{
    public void Configure(EntityTypeBuilder<Domain.UserAggregate.User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(200);
        builder.HasIndex(u => u.Username)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Ignore(u => u.RefreshTokens);
        builder.HasMany(u => u._refreshTokens)
            .WithOne();
        
        builder.Ignore(u => u.ApiKeys);
        builder.HasMany(u => u._apiKeys)
            .WithOne();
    }
}