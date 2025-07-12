using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.File.Dtos;
using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.File.Commands.SaveFile;

public class SaveFileCommandHandler : ICommandHandler<SaveFileCommand, GetFileDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageProviderRepository _fileStorageProviderRepository;
    private readonly ILogger<SaveFileCommandHandler> _logger;

    public SaveFileCommandHandler(IUnitOfWork unitOfWork, IFileStorageProviderRepository fileStorageProviderRepository,
        ILogger<SaveFileCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _fileStorageProviderRepository = fileStorageProviderRepository;
        _logger = logger;
    }


    public async Task<DataResult<GetFileDto>> Handle(
        SaveFileCommand request,
        CancellationToken cancellationToken
    )
    {
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
        var file = new Domain.FileAggregate.File(
            fileStorageScope: request.FileStorageScope,
            storageProvider: storageProviderName,
            path: filePath,
            contentType: request.ContentType,
            id: fileId
        );

        await _unitOfWork
            .FileRepository
            .Add(file);
        await _unitOfWork.SaveChangesAsync();

        var dto = GetFileDto.FromDomain(file);
        return DataResult<GetFileDto>.Success(dto);
    }
}