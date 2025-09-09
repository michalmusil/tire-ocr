using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.Nodes.Dtos.FileReferenceUploader;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Shared.Nodes.Services.FileReferenceUploader;

public interface IFileReferenceUploaderService
{
    public Task<DataResult<FileReference>> UploadFileDataAsync(UploadFileParams input);
}