using TireOcr.Preprocessing.Infrastructure.Models;

namespace TireOcr.Preprocessing.Infrastructure.Extensions;

public static class MlModelExtension
{
    public static string GetAbsolutePath(this MlModel model)
    {
        var rootPath = new DirectoryInfo(AppContext.BaseDirectory).GetSolutionDirectory();
        return Path.Combine(rootPath, model.LocalPath);
    }
}