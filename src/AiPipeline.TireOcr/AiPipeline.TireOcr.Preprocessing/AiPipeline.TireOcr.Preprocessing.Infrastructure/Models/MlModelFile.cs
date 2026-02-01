namespace TireOcr.Preprocessing.Infrastructure.Models;

public class MlModelFile
{
    public required string LocalPath { get; init; }
    public required string DownloadLink { get; init; }
}