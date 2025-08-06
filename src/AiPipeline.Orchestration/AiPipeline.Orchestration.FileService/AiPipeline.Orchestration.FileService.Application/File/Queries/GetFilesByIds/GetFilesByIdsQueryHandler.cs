using AiPipeline.Orchestration.FileService.Application.File.Repositories;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.FileService.Application.File.Queries.GetFilesByIds;

public class GetFilesByIdsQueryHandler : IQueryHandler<GetFilesByIdsQuery, IEnumerable<Domain.FileAggregate.File>>
{
    private readonly IFileRepository _fileEntityRepository;
    private readonly ILogger<GetFilesByIdsQueryHandler> _logger;

    public GetFilesByIdsQueryHandler(IFileRepository fileEntityRepository,
        ILogger<GetFilesByIdsQueryHandler> logger)
    {
        _fileEntityRepository = fileEntityRepository;
        _logger = logger;
    }

    public async Task<DataResult<IEnumerable<Domain.FileAggregate.File>>> Handle(
        GetFilesByIdsQuery request,
        CancellationToken cancellationToken
    )
    {
        var fileIds = request.FileIds.ToArray();
        var foundFiles = (await _fileEntityRepository.GetFilesByIdsAsync(fileIds))
            .ToList();

        if (request.FailIfNotAllFound)
        {
            var notFoundFileIds = fileIds
                .Except(foundFiles.Select(x => x.Id))
                .ToArray();
            if (notFoundFileIds.Any())
                return DataResult<IEnumerable<Domain.FileAggregate.File>>.NotFound(
                    $"Some files were not found. Not found file ids: {string.Join(", ", notFoundFileIds)}");
        }

        return DataResult<IEnumerable<Domain.FileAggregate.File>>.Success(foundFiles);
    }
}