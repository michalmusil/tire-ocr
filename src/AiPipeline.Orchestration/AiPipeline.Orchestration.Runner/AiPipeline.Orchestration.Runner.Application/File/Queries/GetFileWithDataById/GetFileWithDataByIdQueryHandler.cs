using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.File.Dtos;
using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.File.Queries.GetFileWithDataById;

public class GetFileWithDataByIdQueryHandler : IQueryHandler<GetFileWithDataByIdQuery, GetFileWithDataStreamDto>
{
    private readonly IFileRepository _fileRepository;
    private readonly ILogger<GetFileWithDataByIdQueryHandler> _logger;

    public GetFileWithDataByIdQueryHandler(IFileRepository fileRepository,
        ILogger<GetFileWithDataByIdQueryHandler> logger)
    {
        _fileRepository = fileRepository;
        _logger = logger;
    }

    public async Task<DataResult<GetFileWithDataStreamDto>> Handle(
        GetFileWithDataByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var fileResult = await _fileRepository
            .GetFileByIdAsync(
                fileId: request.Id,
                userId: request.UserId
            );
        if (fileResult.IsFailure)
            return DataResult<GetFileWithDataStreamDto>.Failure(fileResult.Failures);

        var dataStreamResult = await _fileRepository.GetFileDataByIdAsync(
            fileId: request.Id,
            userId: request.UserId
        );
        if (dataStreamResult.IsFailure)
            return DataResult<GetFileWithDataStreamDto>.Failure(dataStreamResult.Failures);

        var fileDto = FileDto.FromDomain(fileResult.Data!);
        var resultDto = new GetFileWithDataStreamDto(fileDto, dataStreamResult.Data!);
        return DataResult<GetFileWithDataStreamDto>.Success(resultDto);
    }
}