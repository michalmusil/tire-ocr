using System.Net;
using System.Text.Json;

namespace AiPipeline.Orchestration.Shared.Nodes.Exceptions;

public class HttpRequestExceptionWithContent : HttpRequestException
{
    public string? Content { get; init; }

    public HttpRequestExceptionWithContent(HttpStatusCode code, string? content = null, string? message = null,
        HttpRequestException? innerException = null) : base(message, innerException, code)
    {
        Content = content;
    }

    public bool TryGetContentJsonProperty(string propertyName, out string? jsonProperty)
    {
        jsonProperty = null;
        if (Content == null)
            return false;
        try
        {
            using var document = JsonDocument.Parse(Content);
            var element = document.RootElement.GetProperty(propertyName);
            jsonProperty = element.ToString();
            return true;
        }
        catch
        {
            return false;
        }
    }
}