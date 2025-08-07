using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.File.Dtos;
using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.File.Commands.SaveFile;

public class SaveFileCommandHandler : ICommandHandler<SaveFileCommand, FileDto>
{
    private readonly IFileRepository _fileRepository;
    private readonly ILogger<SaveFileCommandHandler> _logger;

    public SaveFileCommandHandler(IFileRepository fileRepository, ILogger<SaveFileCommandHandler> logger)
    {
        _fileRepository = fileRepository;
        _logger = logger;
    }


    public async Task<DataResult<FileDto>> Handle(
        SaveFileCommand request,
        CancellationToken cancellationToken
    )
    {
        await using var stream = request.FileStream;
        var fileId = request.Id ?? Guid.NewGuid();
        var fileExtension = Path.GetExtension(request.OriginalFileName);
        if (string.IsNullOrEmpty(fileExtension))
            return DataResult<FileDto>.Invalid($"File name {request.OriginalFileName} is invalid.");

        var fileName = $"{fileId}{fileExtension}";
        var saveFileResult = await _fileRepository.Add(
            fileStream: stream,
            contentType: request.ContentType,
            fileName: fileName,
            guid: fileId
        );

        if (saveFileResult.IsFailure)
            return DataResult<FileDto>.Failure(saveFileResult.Failures);

        var dto = FileDto.FromDomain(saveFileResult.Data!);
        return DataResult<FileDto>.Success(dto);
    }
}