using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.NodeSdk.Dtos.FileReferenceUploader;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Shared.NodeSdk.Services.FileReferenceUploader;

public interface IFileReferenceUploaderService
{
    public Task<DataResult<FileReference>> UploadFileDataAsync(UploadFileParams input);
}