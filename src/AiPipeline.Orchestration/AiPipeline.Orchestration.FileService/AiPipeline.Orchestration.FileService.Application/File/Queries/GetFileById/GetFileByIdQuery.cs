using AiPipeline.Orchestration.FileService.Application.File.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.FileService.Application.File.Queries.GetFileById;

public record GetFileByIdQuery(Guid Id) : IQuery<GetFileDto>;