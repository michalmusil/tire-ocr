using System.Text.Json;
using AiPipeline.Orchestration.Contracts.Schema.Converters;

namespace AiPipeline.Orchestration.Runner.Application.Utils;

public static class JsonUtils
{
    public static JsonSerializerOptions GetApElementSerializerOptions() => new()
    {
        Converters = { new ApElementConverter() }
    };
}