using System.Text.Json;
using AiPipeline.Orchestration.Contracts.Schema.Converters;

namespace AiPipeline.Orchestration.Contracts.Tests;

public static class TestUtils
{
    public static readonly JsonSerializerOptions ApElementSerializerOptions = new()
    {
        WriteIndented = true,
        Converters = { new ApElementConverter() }
    };
}