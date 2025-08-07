using AiPipeline.Orchestration.FileService.GrpcSdk.Clients.FileServiceClient;
using AiPipeline.Orchestration.FileService.GrpcServer;
using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using AiPipeline.Orchestration.Runner.Domain.FileAggregate;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Infrastructure.File.Repositories;

public class FileRepositoryGrpc : IFileRepository
{
    private readonly IFileSerivceClient _grpcClient;

    public FileRepositoryGrpc(IFileSerivceClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    public Task<PaginatedCollection<FileValueObject>> GetFilesPaginatedAsync(PaginationParams pagination, FileStorageScope? storageScope = null)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<FileValueObject>> GetFilesByIdsAsync(params Guid[] fileIds)
    {
        throw new NotImplementedException();
    }

    public Task<FileValueObject?> GetFileByIdAsync(Guid fileId)
    {
        throw new NotImplementedException();
    }

    public Task<Stream?> GetFileDataByIdAsync(Guid fileId)
    {
        throw new NotImplementedException();
    }

    public Task<DataResult<FileValueObject>> Add(string fileName, string contentType, Stream fileStream, Guid? guid)
    {
        throw new NotImplementedException();
    }

    public Task<Result> Remove(Guid file)
    {
        throw new NotImplementedException();
    }
}