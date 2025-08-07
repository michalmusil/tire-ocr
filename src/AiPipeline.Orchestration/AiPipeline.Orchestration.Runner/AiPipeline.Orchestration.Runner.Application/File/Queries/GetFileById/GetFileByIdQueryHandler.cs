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
        var foundFile = await _fileRepository
            .GetFileByIdAsync(request.Id);
        if (foundFile is null)
            return DataResult<FileDto>.NotFound($"File with id {request.Id} not found");

        var dto = FileDto.FromDomain(foundFile);
        return DataResult<FileDto>.Success(dto);
    }
}