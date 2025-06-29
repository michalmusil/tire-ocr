using AiPipeline.Orchestration.Runner.Domain.FileAggregate;
using AiPipeline.Orchestration.Shared.Contracts.Schema.Properties;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Application.File.Services;

public interface IFileService
{
    public Task<DataResult<Dictionary<ApFile, Domain.FileAggregate.File>>> GetAllFileEntitiesOfApFilesAsync(
        params ApFile[] apFiles);

    public Task<Result> SaveFileAsync(Stream fileStream, FileStorageScope fileStorageScope, string contentType,
        Guid? id = null);
    
    public Task<Result> RemoveFileAsync(Domain.FileAggregate.File file);
}