using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Shared.Nodes.Services.FileReferenceUploader;

public interface IFileReferenceUploaderService
{
    public Task<Result> UploadTemporaryFileReferenceDataAsync(FileReference reference);
    
    public Task<Result> UploadPermanentFileReferenceDataAsync(FileReference reference);
}