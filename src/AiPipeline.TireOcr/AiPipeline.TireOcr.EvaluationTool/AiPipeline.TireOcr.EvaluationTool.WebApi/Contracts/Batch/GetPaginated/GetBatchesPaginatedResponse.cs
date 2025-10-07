using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using TireOcr.Shared.Pagination;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Batch.GetPaginated;

public record GetBatchesPaginatedResponse(IEnumerable<EvaluationRunBatchLightDto> Items, Pagination Pagination);