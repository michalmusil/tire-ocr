using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.File.Commands.RemoveFile;

public class RemoveFileCommandHandler : ICommandHandler<RemoveFileCommand>
{
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorageProviderRepository _fileStorageProviderRepository;
    private readonly ILogger<RemoveFileCommandHandler> _logger;

    public RemoveFileCommandHandler(IFileRepository fileRepository,
        IFileStorageProviderRepository fileStorageProviderRepository, ILogger<RemoveFileCommandHandler> logger)
    {
        _fileRepository = fileRepository;
        _fileStorageProviderRepository = fileStorageProviderRepository;
        _logger = logger;
    }


    public async Task<Result> Handle(RemoveFileCommand request, CancellationToken cancellationToken)
    {
        var foundFile = await _fileRepository.GetFileByIdAsync(request.Id);
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
        
        await _fileRepository.Remove(foundFile);
        await _fileRepository.SaveChangesAsync();
        return Result.Success();
    }
}