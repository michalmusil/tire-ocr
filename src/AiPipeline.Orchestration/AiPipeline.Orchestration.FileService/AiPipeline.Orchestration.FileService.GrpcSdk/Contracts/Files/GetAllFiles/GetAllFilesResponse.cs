using AiPipeline.Orchestration.FileService.Application.File.Dtos;
using TireOcr.Shared.Pagination;

namespace AiPipeline.Orchestration.FileService.GrpcSdk.Contracts.Files.GetAllFiles;

public record GetAllFilesResponse(IEnumerable<GetFileDto> Items, Pagination Pagination);