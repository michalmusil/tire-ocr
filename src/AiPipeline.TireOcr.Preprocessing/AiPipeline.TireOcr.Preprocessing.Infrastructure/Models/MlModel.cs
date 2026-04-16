namespace TireOcr.Preprocessing.Infrastructure.Models;

public class MlModel
{
    public required string Name { get; init; }
    public required string MainPath { get; init; }
    public required List<MlModelFile> Files { get; init; }
}