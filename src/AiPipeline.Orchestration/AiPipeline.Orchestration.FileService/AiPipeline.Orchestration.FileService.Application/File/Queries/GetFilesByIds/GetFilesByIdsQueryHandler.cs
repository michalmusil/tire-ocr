using AiPipeline.Orchestration.FileService.Application.File.Dtos;
using AiPipeline.Orchestration.FileService.Application.File.Repositories;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.FileService.Application.File.Queries.GetFilesByIds;

public class GetFilesByIdsQueryHandler : IQueryHandler<GetFilesByIdsQuery, IEnumerable<GetFileDto>>
{
    private readonly IFileEntityRepository _fileEntityRepository;
    private readonly ILogger<GetFilesByIdsQueryHandler> _logger;

    public GetFilesByIdsQueryHandler(IFileEntityRepository fileEntityRepository,
        ILogger<GetFilesByIdsQueryHandler> logger)
    {
        _fileEntityRepository = fileEntityRepository;
        _logger = logger;
    }

    public async Task<DataResult<IEnumerable<GetFileDto>>> Handle(
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
                return DataResult<IEnumerable<GetFileDto>>.NotFound(
                    $"Some files were not found. Not found file ids: {string.Join(", ", notFoundFileIds)}");
        }

        var fileDtos = foundFiles
            .Select(GetFileDto.FromDomain)
            .ToList();
        return DataResult<IEnumerable<GetFileDto>>.Success(fileDtos);
    }
}