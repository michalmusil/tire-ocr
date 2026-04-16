using AiPipeline.TireOcr.EvaluationTool.Application.Common.DataAccess;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Queries.GetEvaluationRunById;

public class GetEvaluationRunByIdQueryHandler
    : IQueryHandler<GetEvaluationRunByIdQuery, EvaluationRunDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetEvaluationRunByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DataResult<EvaluationRunDto>> Handle(
        GetEvaluationRunByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var run = await _unitOfWork.EvaluationRunRepository.GetEvaluationRunByIdAsync(
            id: request.Id
        );
        if (run is null)
            return DataResult<EvaluationRunDto>.NotFound($"Evaluation run with id '{request.Id}' not found");

        var dto = EvaluationRunDto.FromDomain(run);
        return DataResult<EvaluationRunDto>.Success(dto);
    }
}