using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.File.Dtos;
using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.File.Queries.GetFileWithDataById;

public class GetFileByIdQueryHandler : IQueryHandler<GetFileWithDataByIdQuery, GetFileWithDataStreamDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageProviderRepository _fileStorageProviderRepository;
    private readonly ILogger<GetFileByIdQueryHandler> _logger;

    public GetFileByIdQueryHandler(IUnitOfWork unitOfWork,
        IFileStorageProviderRepository fileStorageProviderRepository, ILogger<GetFileByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _fileStorageProviderRepository = fileStorageProviderRepository;
    }

    public async Task<DataResult<GetFileWithDataStreamDto>> Handle(
        GetFileWithDataByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var foundFile = await _unitOfWork
            .FileRepository
            .GetFileByIdAsync(request.Id);
        if (foundFile is null)
            return DataResult<GetFileWithDataStreamDto>.NotFound($"File with id {request.Id} not found");

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