using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos;
using TireOcr.Shared.Pagination;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.EvaluationRunBatch.Contracts.Batch.GetPaginated;

public record GetBatchesPaginatedResponse(IEnumerable<EvaluationRunBatchLightDto> Items, Pagination Pagination);