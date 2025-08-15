using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.File.Dtos;
using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.File.Queries.GetFileById;

public class GetFileByIdQueryHandler : IQueryHandler<GetFileByIdQuery, FileDto>
{
    private readonly IFileRepository _fileRepository;
    private readonly ILogger<GetFileByIdQueryHandler> _logger;

    public GetFileByIdQueryHandler(IFileRepository fileRepository,
        ILogger<GetFileByIdQueryHandler> logger)
    {
        _fileRepository = fileRepository;
        _logger = logger;
    }

    public async Task<DataResult<FileDto>> Handle(
        GetFileByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var fileResult = await _fileRepository
            .GetFileByIdAsync(
                fileId: request.Id,
                userId: request.UserId
            );
        if (fileResult.IsFailure)
            return DataResult<FileDto>.Failure(fileResult.Failures);

        var dto = FileDto.FromDomain(fileResult.Data!);
        return DataResult<FileDto>.Success(dto);
    }
}