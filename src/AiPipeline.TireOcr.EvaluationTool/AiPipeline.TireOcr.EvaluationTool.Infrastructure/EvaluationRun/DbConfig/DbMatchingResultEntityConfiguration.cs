using System.Text.Json;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.DbConfig;

public class DbMatchingResultEntityConfiguration : IEntityTypeConfiguration<DbMatchingResultEntity>
{
    private readonly ValueConverter<List<TireDbMatchValueObject>, string> _matchesConverter = new(
        matches => JsonSerializer
            .Serialize(matches, JsonSerializerOptions.Default),
        jsonString => JsonSerializer
            .Deserialize<List<TireDbMatchValueObject>>(jsonString, JsonSerializerOptions.Default)!
    );

    public void Configure(EntityTypeBuilder<DbMatchingResultEntity> builder)
    {
        builder.HasKey(dmr => dmr.Id);

        builder.Property(dmr => dmr.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(dmr => dmr.RunId)
            .IsRequired();

        builder.Property(dmr => dmr.ManufacturerMatch)
            .IsRequired(false);

        builder.Property(dmr => dmr.DurationMs)
            .IsRequired();

        builder.Property(dmr => dmr.Matches)
            .IsRequired()
            .HasConversion(_matchesConverter);

        builder.Property(dmr => dmr.CreatedAt)
            .IsRequired();
        
        builder.Property(dmr => dmr.UpdatedAt)
            .IsRequired();
    }
}