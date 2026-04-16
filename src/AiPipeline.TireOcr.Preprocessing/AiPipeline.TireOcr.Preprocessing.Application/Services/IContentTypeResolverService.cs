namespace TireOcr.Preprocessing.Application.Services;

public interface IContentTypeResolverService
{
    public bool IsContentTypeSupported(string contentType);
}