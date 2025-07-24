using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Shared.Nodes.Services.FileReferenceUploader;

public interface IFileReferenceUploaderService
{
    public Task<DataResult<FileReference>> UploadFileDataAsync(Guid fileId, Stream fileStream, string contentType,
        string fileName, bool storePermanently);
}