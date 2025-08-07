using AiPipeline.Orchestration.FileService.Application.File.Repositories;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.FileService.Application.File.Commands.RemoveFile;

public class RemoveFileCommandHandler : ICommandHandler<RemoveFileCommand>
{
    private readonly IFileEntityRepository _fileEntityRepository;
    private readonly IFileStorageProviderRepository _fileStorageProviderRepository;
    private readonly ILogger<RemoveFileCommandHandler> _logger;

    public RemoveFileCommandHandler(IFileEntityRepository fileEntityRepository,
        IFileStorageProviderRepository fileStorageProviderRepository, ILogger<RemoveFileCommandHandler> logger)
    {
        _fileEntityRepository = fileEntityRepository;
        _fileStorageProviderRepository = fileStorageProviderRepository;
        _logger = logger;
    }


    public async Task<Result> Handle(RemoveFileCommand request, CancellationToken cancellationToken)
    {
        var foundFile = await _fileEntityRepository
            .GetFileByIdAsync(request.Id);
        if (foundFile is null)
            return Result.NotFound($"File with id {request.Id} was not found");

        var fileName = Path.GetFileName(foundFile.Path);
        var removed = await _fileStorageProviderRepository
            .RemoveFileAsync(
                scope: foundFile.FileStorageScope,
                fileName: fileName
            );
        if (!removed)
            return Result.Failure(new Failure(500, "Failed to remove file from storage"));

        await _fileEntityRepository
            .Remove(foundFile);
        await _fileEntityRepository.SaveChangesAsync();
        return Result.Success();
    }
}