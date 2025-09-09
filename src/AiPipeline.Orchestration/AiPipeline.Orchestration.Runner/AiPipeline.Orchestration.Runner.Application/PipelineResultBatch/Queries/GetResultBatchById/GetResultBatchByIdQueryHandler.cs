using AiPipeline.Orchestration.Runner.Application.PipelineResult.Repositories;
using AiPipeline.Orchestration.Runner.Application.PipelineResultBatch.Dtos;
using AiPipeline.Orchestration.Runner.Application.PipelineResultBatch.Repositories;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResultBatch.Queries.GetResultBatchById;

public class GetResultBatchByIdQueryHandler : IQueryHandler<GetResultBatchByIdQuery, GetPipelineResultBatchDto>
{
    private readonly IPipelineResultBatchEntityRepository _resultBatchRepository;
    private readonly IPipelineResultEntityRepository _resultRepository;

    public GetResultBatchByIdQueryHandler(IPipelineResultBatchEntityRepository resultBatchRepository,
        IPipelineResultEntityRepository resultRepository)
    {
        _resultBatchRepository = resultBatchRepository;
        _resultRepository = resultRepository;
    }

    public async Task<DataResult<GetPipelineResultBatchDto>> Handle(GetResultBatchByIdQuery request,
        CancellationToken cancellationToken)
    {
        var batch = await _resultBatchRepository.GetResultBatchByIdAsync(request.Id);
        if (batch is null)
            return DataResult<GetPipelineResultBatchDto>.NotFound($"Result batch with id '{request.Id}' not found");

        if (batch.UserId != request.UserId)
            return DataResult<GetPipelineResultBatchDto>.Forbidden(
                $"User '{request.UserId}' is not authorized to access batch '{request.Id}'"
            );

        var results = (await _resultRepository
                .GetPipelineResultsByBatchIdAsync(request.Id))
            .ToList();

        var dto = GetPipelineResultBatchDto.FromDomain(batch, results);
        return DataResult<GetPipelineResultBatchDto>.Success(dto);
    }
}