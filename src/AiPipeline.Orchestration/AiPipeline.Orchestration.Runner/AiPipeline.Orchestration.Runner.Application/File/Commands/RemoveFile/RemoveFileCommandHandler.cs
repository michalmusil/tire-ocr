using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.File.Commands.RemoveFile;

public class RemoveFileCommandHandler : ICommandHandler<RemoveFileCommand>
{
    private readonly IFileRepository _fileRepository;

    public RemoveFileCommandHandler(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }


    public Task<Result> Handle(RemoveFileCommand request, CancellationToken cancellationToken) =>
        _fileRepository.Remove(fileId: request.Id, userId: request.UserId);
}