using AiPipeline.Orchestration.Runner.Application.File.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.File.Queries.GetFileById;

public record GetFileByIdQuery(Guid Id) : IQuery<FileDto>;