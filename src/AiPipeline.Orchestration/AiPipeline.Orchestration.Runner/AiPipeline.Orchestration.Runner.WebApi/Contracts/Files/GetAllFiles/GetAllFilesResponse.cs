using AiPipeline.Orchestration.Runner.Application.File.Dtos;
using TireOcr.Shared.Pagination;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Files.GetAllFiles;

public record GetAllFilesResponse(IEnumerable<GetFileDto> Items, Pagination Pagination);