using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using AiPipeline.Orchestration.Shared.Contracts.Schema.Properties;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.File.Queries.GetAllFileEntitiesOfApFiles;

public class GetAvailableNodesQueryHandler : IQueryHandler<GetAllFileEntitiesOfApFiles,
    Dictionary<ApFile, Domain.FileAggregate.File>>
{
    private readonly IFileRepository _fileRepository;
    private readonly ILogger<GetAvailableNodesQueryHandler> _logger;

    public GetAvailableNodesQueryHandler(IFileRepository fileRepository, ILogger<GetAvailableNodesQueryHandler> logger)
    {
        _fileRepository = fileRepository;
        _logger = logger;
    }

    public async Task<DataResult<Dictionary<ApFile, Domain.FileAggregate.File>>> Handle(
        GetAllFileEntitiesOfApFiles request,
        CancellationToken cancellationToken
    )
    {
        var fileIds = request.ApFiles
            .Select(x => x.Id)
            .Distinct()
            .ToArray();

        var foundFiles = (await _fileRepository.GetFilesByIdsAsync(fileIds))
            .ToList();

        var notFoundFileIds = fileIds
            .Except(foundFiles.Select(x => x.Id))
            .ToArray();
        if (notFoundFileIds.Any())
            return DataResult<Dictionary<ApFile, Domain.FileAggregate.File>>.NotFound(
                $"Some files were not found. Not found file ids: {string.Join(", ", notFoundFileIds)}");

        var resultDictionary = new Dictionary<ApFile, Domain.FileAggregate.File>();
        foreach (var id in fileIds)
        {
            var apFile = request.ApFiles.First(x => x.Id == id);
            var file = foundFiles.First(x => x.Id == id);
            resultDictionary.Add(apFile, file);
        }

        return DataResult<Dictionary<ApFile, Domain.FileAggregate.File>>.Success(resultDictionary);
    }
}