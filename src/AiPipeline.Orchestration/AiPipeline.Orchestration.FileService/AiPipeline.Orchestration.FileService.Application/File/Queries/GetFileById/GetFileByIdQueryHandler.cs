using AiPipeline.Orchestration.FileService.Application.File.Dtos;
using AiPipeline.Orchestration.FileService.Application.File.Repositories;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.FileService.Application.File.Queries.GetFileById;

public class GetFileByIdQueryHandler : IQueryHandler<GetFileByIdQuery, GetFileDto>
{
    private readonly IFileRepository _fileEntityRepository;
    private readonly ILogger<GetFileByIdQueryHandler> _logger;

    public GetFileByIdQueryHandler(IFileRepository fileEntityRepository,
        ILogger<GetFileByIdQueryHandler> logger)
    {
        _fileEntityRepository = fileEntityRepository;
        _logger = logger;
    }

    public async Task<DataResult<GetFileDto>> Handle(
        GetFileByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var foundFile = await _fileEntityRepository
            .GetFileByIdAsync(request.Id);
        if (foundFile is null)
            return DataResult<GetFileDto>.NotFound($"File with id {request.Id} not found");

        var dto = GetFileDto.FromDomain(foundFile);
        return DataResult<GetFileDto>.Success(dto);
    }
}