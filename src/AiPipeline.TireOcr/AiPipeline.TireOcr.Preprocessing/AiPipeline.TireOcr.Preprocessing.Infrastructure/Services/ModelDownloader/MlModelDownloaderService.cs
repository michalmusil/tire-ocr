using Microsoft.Extensions.Logging;
using TireOcr.Preprocessing.Infrastructure.Extensions;
using TireOcr.Preprocessing.Infrastructure.Models;
using TireOcr.Shared.Result;

namespace TireOcr.Preprocessing.Infrastructure.Services.ModelDownloader;

public class MlModelDownloaderService : IMlModelDownloaderService
{
    private readonly HttpClient _client;
    private readonly ILogger<MlModelDownloaderService> _logger;

    public MlModelDownloaderService(HttpClient client, ILogger<MlModelDownloaderService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<bool> DownloadAsync(MlModel model)
    {
        var nonDownloadedFiles = model.Files.Where(f => !File.Exists(f.GetAbsLocalPath()));
        foreach (var file in nonDownloadedFiles)
        {
            _logger.LogInformation(
                $"Downloading file '{file.LocalPath}' of model '{model.Name}' from '{file.DownloadLink}'.");
            var fileDirectory = Path.GetDirectoryName(file.GetAbsLocalPath());
            if (fileDirectory is null)
                throw new ApplicationException($"Directory of file '{file.LocalPath}' is not valid.");

            Directory.CreateDirectory(fileDirectory);
            try
            {
                using var response = await _client.GetAsync(file.DownloadLink);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to download model one of {model.Name}'s files.");
                    return false;
                }

                await using var fileStream =
                    new FileStream(file.GetAbsLocalPath(), FileMode.Create, FileAccess.Write, FileShare.None);
                await response.Content.CopyToAsync(fileStream);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to download model one of {model.Name}'s files.");
                return false;
            }
        }

        return true;
    }
}