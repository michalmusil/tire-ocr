using AiPipeline.Orchestration.FileService.Application.File.Dtos;
using AiPipeline.Orchestration.FileService.Domain.FileAggregate;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.FileService.Application.File.Commands.SaveFile;

public record SaveFileCommand(
    Stream FileStream,
    FileStorageScope FileStorageScope,
    string ContentType,
    string OriginalFileName,
    Guid? Id = null
) : ICommand<GetFileDto>;