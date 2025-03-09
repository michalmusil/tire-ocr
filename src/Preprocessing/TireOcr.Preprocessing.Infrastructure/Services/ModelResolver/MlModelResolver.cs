using Microsoft.Extensions.Configuration;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Infrastructure.Extensions;
using TireOcr.Preprocessing.Infrastructure.Models;
using TireOcr.Preprocessing.Infrastructure.Services.ModelDownloader;

namespace TireOcr.Preprocessing.Infrastructure.Services.ModelResolver;

public class MlModelResolver : IMlModelResolver
{
    private readonly IMlModelDownloader _modelDownloader;
    private readonly IConfiguration _configuration;
    private readonly Dictionary<Type, Func<MlModel?>> _factories;

    public MlModelResolver(IConfiguration configuration, IMlModelDownloader modelDownloader)
    {
        _configuration = configuration;
        _modelDownloader = modelDownloader;
        _factories = new()
        {
            { typeof(ITireDetectionService), () => GetModel("TireSegmentation") },
            { typeof(ITextDetectionService), () => GetModel("StripCharacterDetection") }
        };
    }

    public MlModel? Resolve<T>()
    {
        var found = _factories.TryGetValue(typeof(T), out var factory);
        if (!found)
            return null;

        return factory!();
    }

    public async Task EnsureAllModelsLoadedAsync()
    {
        var availableModels = GetAllAvailableModels();
        await Task.WhenAll(availableModels.Select(EnsureMlModelLoadedAsync));
    }

    private IEnumerable<MlModel> GetAllAvailableModels()
    {
        var availableModels = _configuration.GetSection("MlModels").Get<List<MlModel>>();
        if (availableModels is null)
            throw new ApplicationException("MlModels are missing in configuration");
        return availableModels;
    }

    private MlModel GetModel(string name)
    {
        var availableModels = GetAllAvailableModels();
        var model = availableModels.FirstOrDefault(m => m.Name == name);
        if (model is null)
            throw new ApplicationException($"MlModel {name} not found in configuration");

        return model;
    }

    private async Task EnsureMlModelLoadedAsync(MlModel model)
    {
        var modelAbsolutePath = model.GetAbsolutePath();
        if (File.Exists(modelAbsolutePath))
            return;

        var modelDirectory = Path.GetDirectoryName(modelAbsolutePath);
        if (modelDirectory is null)
            throw new ApplicationException($"Directory of MlModel {modelAbsolutePath} is not valid.");

        Directory.CreateDirectory(modelDirectory);
        var downloaded = await _modelDownloader.DownloadAsync(model);
        if (!downloaded)
            throw new ApplicationException($"MlModel {model.Name} could not be downloaded.");
    }
}