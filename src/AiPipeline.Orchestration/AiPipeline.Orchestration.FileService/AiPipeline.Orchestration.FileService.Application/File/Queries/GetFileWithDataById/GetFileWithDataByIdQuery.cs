using AiPipeline.Orchestration.FileService.Application.File.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.FileService.Application.File.Queries.GetFileWithDataById;

public record GetFileWithDataByIdQuery(Guid Id) : IQuery<GetFileWithDataStreamDto>;