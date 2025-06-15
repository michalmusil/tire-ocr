using System.Text.Json;
using AiPipeline.Orchestration.Shared.Contracts.Schema.Converters;

namespace AiPipeline.Orchestration.Runner.Application.Common.Utils;

public static class JsonUtils
{
    public static JsonSerializerOptions GetApElementSerializerOptions() => new()
    {
        Converters = { new ApElementConverter() }
    };
}