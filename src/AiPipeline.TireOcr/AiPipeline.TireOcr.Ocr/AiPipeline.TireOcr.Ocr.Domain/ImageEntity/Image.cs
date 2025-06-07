namespace TireOcr.Ocr.Domain.ImageEntity;

public class Image
{
    public byte[] Data { get; }
    public string Name { get; }
    public string ContentType { get; }

    public Image(byte[] data, string name, string contentType)
    {
        Data = data;
        Name = name;
        ContentType = contentType;
    }
}