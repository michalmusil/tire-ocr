using System.Text.Json;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Converters;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Common.Utils;

public static class JsonUtils
{
    public static JsonSerializerOptions GetApElementSerializerOptions() => new()
    {
        Converters = { new ApElementConverter() },
        PropertyNameCaseInsensitive = true
    };

    public static ValueConverter<IApElement, string> GetApElementJsonValueConverter()
    {
        return new(
            apElement => JsonSerializer
                .Serialize(apElement, GetApElementSerializerOptions()),
            jsonString => JsonSerializer
                .Deserialize<IApElement>(jsonString, GetApElementSerializerOptions())!
        );
    }

    public static ValueConverter<T, string> GetDefaultJsonValueConverter<T>()
    {
        return new(
            value => JsonSerializer.Serialize(value, JsonSerializerOptions.Default),
            jsonString => JsonSerializer.Deserialize<T>(jsonString, JsonSerializerOptions.Default)!
        );
    }
}