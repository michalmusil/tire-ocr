using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Shared.Nodes.Services.FileReferenceDownloader;

public interface IFileReferenceDownloaderService
{
    public Task<DataResult<Stream>> DownloadFileReferenceDataAsync(FileReference reference);
}