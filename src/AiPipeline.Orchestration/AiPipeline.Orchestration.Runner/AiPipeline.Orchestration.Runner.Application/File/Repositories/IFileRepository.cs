using AiPipeline.Orchestration.Runner.Domain.FileAggregate;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Application.File.Repositories;

public interface IFileRepository
{
    public Task<PaginatedCollection<FileValueObject>> GetFilesPaginatedAsync(
        PaginationParams pagination, FileStorageScope? storageScope = null
    );

    public Task<IEnumerable<FileValueObject>> GetFilesByIdsAsync(params Guid[] fileIds);
    public Task<FileValueObject?> GetFileByIdAsync(Guid fileId);
    public Task<Stream?> GetFileDataByIdAsync(Guid fileId);
    public Task<DataResult<FileValueObject>> Add(string fileName, string contentType, Stream fileStream, Guid? guid);
    public Task<Result> Remove(Guid file);
}