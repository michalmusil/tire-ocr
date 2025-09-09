using AiPipeline.Orchestration.Runner.Application.File.Dtos;
using AiPipeline.Orchestration.Runner.Domain.FileAggregate;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.File.Commands.SaveFile;

public record SaveFileCommand(
    Stream FileStream,
    Guid UserId,
    FileStorageScope FileStorageScope,
    string ContentType,
    string OriginalFileName,
    Guid? Id = null
) : ICommand<FileDto>;