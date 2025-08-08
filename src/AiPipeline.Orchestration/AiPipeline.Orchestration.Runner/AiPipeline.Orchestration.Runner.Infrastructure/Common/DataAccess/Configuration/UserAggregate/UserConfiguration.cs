using AiPipeline.Orchestration.Runner.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Common.DataAccess.Configuration.UserAggregate;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Ignore(u => u.RefreshTokens);
        builder.HasMany(u => u._refreshTokens)
            .WithOne();
    }
}