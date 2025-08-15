using AiPipeline.Orchestration.FileService.Application.File.Dtos;
using AiPipeline.Orchestration.FileService.Application.File.Repositories;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.FileService.Application.File.Queries.GetFileWithDataById;

public class GetFileWithDataByIdQueryHandler : IQueryHandler<GetFileWithDataByIdQuery, GetFileWithDataStreamDto>
{
    private readonly IFileEntityRepository _fileEntityRepository;
    private readonly IFileStorageProviderRepository _fileStorageProviderRepository;
    private readonly ILogger<GetFileWithDataByIdQueryHandler> _logger;

    public GetFileWithDataByIdQueryHandler(IFileEntityRepository fileEntityRepository,
        IFileStorageProviderRepository fileStorageProviderRepository, ILogger<GetFileWithDataByIdQueryHandler> logger)
    {
        _fileEntityRepository = fileEntityRepository;
        _logger = logger;
        _fileStorageProviderRepository = fileStorageProviderRepository;
    }

    public async Task<DataResult<GetFileWithDataStreamDto>> Handle(
        GetFileWithDataByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var foundFile = await _fileEntityRepository
            .GetFileByIdAsync(request.Id);
        if (foundFile is null)
            return DataResult<GetFileWithDataStreamDto>.NotFound($"File with id {request.Id} not found");

        if (foundFile.UserId != request.UserId)
            return DataResult<GetFileWithDataStreamDto>.Forbidden(
                $"User '{request.UserId}' is not authorized to access file '{request.Id}'");

        var fileName = Path.GetFileName(foundFile.Path);
        var dataStream = await _fileStorageProviderRepository.DownloadFileAsync(
            scope: foundFile.FileStorageScope,
            fileName: fileName
        );
        if (dataStream is null)
            return DataResult<GetFileWithDataStreamDto>.Failure(
                new Failure(500, $"Failed to retrieve stored file data for {request.Id}")
            );

        var fileDto = GetFileDto.FromDomain(foundFile);
        var resultDto = new GetFileWithDataStreamDto(fileDto, dataStream);
        return DataResult<GetFileWithDataStreamDto>.Success(resultDto);
    }
}