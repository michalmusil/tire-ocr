using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Shared.Contracts.Schema.Properties;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.File.Queries.GetAllFileEntitiesOfApFiles;

public class GetAllFileEntitiesOfApFilesQueryHandler : IQueryHandler<GetAllFileEntitiesOfApFilesQuery,
    Dictionary<ApFile, Domain.FileAggregate.File>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetAllFileEntitiesOfApFilesQueryHandler> _logger;

    public GetAllFileEntitiesOfApFilesQueryHandler(IUnitOfWork unitOfWork,
        ILogger<GetAllFileEntitiesOfApFilesQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<DataResult<Dictionary<ApFile, Domain.FileAggregate.File>>> Handle(
        GetAllFileEntitiesOfApFilesQuery request,
        CancellationToken cancellationToken
    )
    {
        var fileIds = request.ApFiles
            .Select(x => x.Id)
            .Distinct()
            .ToArray();

        var foundFiles = (await _unitOfWork.FileRepository.GetFilesByIdsAsync(fileIds))
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