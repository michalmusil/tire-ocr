namespace TireOcr.Preprocessing.Infrastructure.Extensions;

public static class DirectoryExtension
{
    public static string GetSolutionDirectory(this DirectoryInfo directory)
    {
        var currentDirectory = directory.FullName;
        var maxDepth = 15;
        var depthCounter = 0;
        while (!string.IsNullOrEmpty(currentDirectory) && depthCounter < maxDepth)
        {
            if (Directory.GetFiles(currentDirectory, "appsettings*.json").Any())
            {
                return currentDirectory;
            }

            depthCounter++;
            currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
        }

        throw new InvalidOperationException("Solution root could not be found.");
    }
}