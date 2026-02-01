using TireOcr.Preprocessing.Infrastructure.Models;

namespace TireOcr.Preprocessing.Infrastructure.Extensions;

public static class MlModelFileExtension
{
    public static string GetAbsLocalPath(this MlModelFile file)
    {
        var rootPath = new DirectoryInfo(AppContext.BaseDirectory).GetSolutionDirectory();
        return Path.Combine(rootPath, file.LocalPath);
    }
}