namespace TireOcr.RunnerPrototype.Models;

public record Image(
    byte[] Data,
    string FileName,
    string ContentType
);