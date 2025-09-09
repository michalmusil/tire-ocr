using AiPipeline.Orchestration.Runner.Application.NodeType.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.NodeType.Commands;

public record SaveNodeTypeCommand(SaveNodeDto Dto) : ICommand<GetNodeDto>;