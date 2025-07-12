using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.File.Dtos;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.File.Queries.GetFileById;

public class GetFileByIdQueryHandler : IQueryHandler<GetFileByIdQuery, GetFileDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetFileByIdQueryHandler> _logger;

    public GetFileByIdQueryHandler(IUnitOfWork unitOfWork,
        ILogger<GetFileByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<DataResult<GetFileDto>> Handle(
        GetFileByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var foundFile = await _unitOfWork
            .FileRepository
            .GetFileByIdAsync(request.Id);
        if (foundFile is null)
            return DataResult<GetFileDto>.NotFound($"File with id {request.Id} not found");

        var dto = GetFileDto.FromDomain(foundFile);
        return DataResult<GetFileDto>.Success(dto);
    }
}