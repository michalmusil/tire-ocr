namespace TireOcr.Preprocessing.Application.Services;

public interface IContentTypeResolver
{
    public bool IsContentTypeSupported(string contentType);
}