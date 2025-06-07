namespace TireOcr.Preprocessing.Domain.ImageEntity;

public class Image
{
    public byte[] Data { get; set; }
    public string Name { get; set; }
    public ImageSize Size { get; set; }

    public Image(byte[] data, string name, ImageSize size)
    {
        Data = data;
        Name = name;
        Size = size;
    }
}