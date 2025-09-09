using System.Text.Json;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Converters;

namespace AiPipeline.Orchestration.Shared.Tests;

public static class TestUtils
{
    public static readonly JsonSerializerOptions ApElementSerializerOptions = new()
    {
        WriteIndented = true,
        Converters = { new ApElementConverter() }
    };
}