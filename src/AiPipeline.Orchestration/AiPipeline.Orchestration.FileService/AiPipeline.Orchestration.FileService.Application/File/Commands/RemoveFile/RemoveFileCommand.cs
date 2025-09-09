using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.FileService.Application.File.Commands.RemoveFile;

public record RemoveFileCommand(Guid Id, Guid UserId) : ICommand;