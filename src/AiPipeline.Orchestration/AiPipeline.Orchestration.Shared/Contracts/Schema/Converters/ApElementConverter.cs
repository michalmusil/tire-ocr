using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using AiPipeline.Orchestration.Shared.Contracts.Schema.Attributes;

namespace AiPipeline.Orchestration.Shared.Contracts.Schema.Converters;

public class ApElementConverter : JsonConverter<IApElement>
{
    private static readonly Dictionary<string, Type> NameToType = new();
    private static readonly Dictionary<Type, string> TypeToName = new();

    static ApElementConverter()
    {
        var baseApElementType = typeof(IApElement);
        var apElementSubtypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => baseApElementType.IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false })
            .ToList();

        foreach (var type in apElementSubtypes)
        {
            var attr = type.GetCustomAttribute<ApElementTypeAttribute>();
            if (attr != null)
            {
                NameToType[attr.TypeDiscriminator] = type;
                TypeToName[type] = attr.TypeDiscriminator;
            }
        }
    }

    public override IApElement? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (!root.TryGetProperty("type", out var typeProp))
            throw new JsonException(
                "'type' property not found. ApElement can't be deserialized without the 'type' property.");

        var typeName = typeProp.GetString();

        if (typeName == null || !NameToType.TryGetValue(typeName, out var targetType))
            throw new JsonException($"Unknown ApElement type '{typeName}'.");

        return (IApElement?)JsonSerializer.Deserialize(root.GetRawText(), targetType, options);
    }

    public override void Write(Utf8JsonWriter writer, IApElement value, JsonSerializerOptions options)
    {
        var type = value.GetType();
        if (!TypeToName.TryGetValue(type, out var typeName))
            throw new JsonException($"No ApElementTypeAttribute found for {type.Name}");

        var json = JsonSerializer.SerializeToElement(value, type, options);

        writer.WriteStartObject();
        writer.WriteString("type", typeName);

        foreach (var property in json.EnumerateObject())
        {
            property.WriteTo(writer);
        }

        writer.WriteEndObject();
    }
}