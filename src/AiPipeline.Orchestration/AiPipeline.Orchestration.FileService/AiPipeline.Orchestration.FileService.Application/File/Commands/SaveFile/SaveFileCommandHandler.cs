using AiPipeline.Orchestration.FileService.Application.File.Dtos;
using AiPipeline.Orchestration.FileService.Application.File.Repositories;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.FileService.Application.File.Commands.SaveFile;

public class SaveFileCommandHandler : ICommandHandler<SaveFileCommand, GetFileDto>
{
    private readonly IFileEntityRepository _fileEntityRepository;
    private readonly IFileStorageProviderRepository _fileStorageProviderRepository;
    private readonly ILogger<SaveFileCommandHandler> _logger;

    public SaveFileCommandHandler(IFileEntityRepository fileEntityRepository,
        IFileStorageProviderRepository fileStorageProviderRepository,
        ILogger<SaveFileCommandHandler> logger)
    {
        _fileEntityRepository = fileEntityRepository;
        _fileStorageProviderRepository = fileStorageProviderRepository;
        _logger = logger;
    }


    public async Task<DataResult<GetFileDto>> Handle(
        SaveFileCommand request,
        CancellationToken cancellationToken
    )
    {
        if (request.Id is not null)
        {
            var existingFile = await _fileEntityRepository.GetFileByIdAsync(request.Id.Value);
            if (existingFile is not null)
                return DataResult<GetFileDto>.Conflict($"File with id '{request.Id}' already exists");
        }

        await using var stream = request.FileStream;
        var fileId = request.Id ?? Guid.NewGuid();
        var fileExtension = Path.GetExtension(request.OriginalFileName);
        if (string.IsNullOrEmpty(fileExtension))
            return DataResult<GetFileDto>.Invalid($"File name {request.OriginalFileName} is invalid.");

        var fileName = $"{fileId}{fileExtension}";
        var saved = await _fileStorageProviderRepository.UploadFileAsync(
            fileStream: stream,
            contentType: request.ContentType,
            scope: request.FileStorageScope,
            fileName: fileName
        );

        if (!saved)
            return DataResult<GetFileDto>.Failure(new Failure(500, "Failed to store file data"));

        var storageProviderName = _fileStorageProviderRepository.GetProviderName();
        var filePath = _fileStorageProviderRepository.GetFullFilePathFor(request.FileStorageScope, fileName);
        var file = new FileService.Domain.FileAggregate.File(
            id: fileId,
            userId: request.UserId,
            fileStorageScope: request.FileStorageScope,
            storageProvider: storageProviderName,
            path: filePath,
            contentType: request.ContentType
        );

        await _fileEntityRepository
            .Add(file);
        await _fileEntityRepository.SaveChangesAsync();

        var dto = GetFileDto.FromDomain(file);
        return DataResult<GetFileDto>.Success(dto);
    }
}