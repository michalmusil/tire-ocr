namespace TireOcr.Preprocessing.Infrastructure.Models;

public class MlModel
{
    public required string Name { get; init; }
    public required string LocalPath { get; init; }
    public required string DownloadLink { get; init; }
}