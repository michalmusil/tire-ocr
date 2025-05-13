using Microsoft.Extensions.Configuration;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Infrastructure.Extensions;
using TireOcr.Preprocessing.Infrastructure.Models;
using TireOcr.Preprocessing.Infrastructure.Services.ModelDownloader;
using TireOcr.Shared.Result;

namespace TireOcr.Preprocessing.Infrastructure.Services.ModelResolver;

public class MlModelResolver : IMlModelResolver
{
    private const int MaxNumberOfDownloadRetries = 3;
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

    public async Task<DataResult<MlModel>> Resolve<T>()
    {
        var allModelsLoadedResult = await EnsureAllModelsLoadedAsync();
        if (allModelsLoadedResult.IsFailure)
            return DataResult<MlModel>.Failure(allModelsLoadedResult.Failures);

        var found = _factories.TryGetValue(typeof(T), out var factory);
        if (!found)
            return DataResult<MlModel>.NotFound($"Failed to resolve MlModelFactory for {typeof(T).Name}");

        var model = factory!();
        if (model == null)
            return DataResult<MlModel>.NotFound($"Model not found for type: {typeof(T).Name}");
        return DataResult<MlModel>.Success(model);
    }

    public async Task<Result> EnsureAllModelsLoadedAsync()
    {
        var availableModels = GetAllAvailableModels();
        var results = await Task.WhenAll(availableModels.Select(EnsureMlModelLoadedAsync));
        if (results.Any(r => r.IsFailure))
            return Result.Failure(new Failure(500, "Failed to download all necessary ML models"));

        return Result.Success();
    }

    private IEnumerable<MlModel> GetAllAvailableModels()
    {
        var availableModels = _configuration.GetSection("MlModels").Get<List<MlModel>>();
        if (availableModels is null)
            throw new ApplicationException("MlModels are missing in configuration");
        return availableModels;
    }

    private MlModel? GetModel(string name)
    {
        var availableModels = GetAllAvailableModels();
        var model = availableModels.FirstOrDefault(m => m.Name == name);

        return model;
    }

    private async Task<Result> EnsureMlModelLoadedAsync(MlModel model)
    {
        var retryCount = 0;
        while (retryCount <= MaxNumberOfDownloadRetries)
        {
            var modelAbsolutePath = model.GetAbsolutePath();
            if (File.Exists(modelAbsolutePath))
                return Result.Success();

            var modelDirectory = Path.GetDirectoryName(modelAbsolutePath);
            if (modelDirectory is null)
                throw new ApplicationException($"Directory of MlModel {modelAbsolutePath} is not valid.");

            Directory.CreateDirectory(modelDirectory);
            var downloaded = await _modelDownloader.DownloadAsync(model);
            if (downloaded)
                return Result.Success();
            retryCount++;
        }

        return Result.Failure();
    }
}