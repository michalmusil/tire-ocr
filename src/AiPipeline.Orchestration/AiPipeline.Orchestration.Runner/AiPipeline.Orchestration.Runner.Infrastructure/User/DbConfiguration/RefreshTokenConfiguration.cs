using AiPipeline.Orchestration.Runner.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiPipeline.Orchestration.Runner.Infrastructure.User.DbConfiguration;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Token);
        builder.Property(rt => rt.Token)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(rt => rt.UserId)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(rt => rt.Token)
            .IsRequired();
        builder.Property(rt => rt.ExpiresAt)
            .IsRequired();
        builder.Property(rt => rt.Created)
            .IsRequired();
        builder.Property(rt => rt.Invalidated)
            .IsRequired();

        builder.HasOne<Domain.UserAggregate.User>()
            .WithMany(u => u._refreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}