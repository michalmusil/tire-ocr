using AiPipeline.Orchestration.Runner.Domain.FileAggregate;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Application.File.Repositories;

public interface IFileRepository
{
    public Task<PaginatedCollection<FileValueObject>> GetFilesPaginatedAsync(
        PaginationParams pagination, Guid userId, FileStorageScope? storageScope = null
    );

    public Task<DataResult<IEnumerable<FileValueObject>>> GetFilesByIdsAsync(Guid userId, params Guid[] fileIds);
    public Task<DataResult<FileValueObject>> GetFileByIdAsync(Guid fileId, Guid userId);
    public Task<DataResult<Stream>> GetFileDataByIdAsync(Guid fileId, Guid userId);

    public Task<DataResult<FileValueObject>> Add(Guid userId, string fileName, string contentType, Stream fileStream,
        FileStorageScope? storageScope, Guid? guid);

    public Task<Result> Remove(Guid fileId, Guid userId);
}