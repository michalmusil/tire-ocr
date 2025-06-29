using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.File.Commands.RemoveFile;

public record RemoveFileCommand(Guid Id) : ICommand;