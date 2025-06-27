using AiPipeline.Orchestration.Runner.Application.NodeType.Dtos;
using AiPipeline.Orchestration.Runner.Application.NodeType.Repositories;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.NodeType.Commands;

public class SaveNodeTypeCommandHandler : ICommandHandler<SaveNodeTypeCommand, GetNodeDto>
{
    private readonly INodeTypeRepository _nodeTypeRepository;
    private readonly ILogger<SaveNodeTypeCommandHandler> _logger;

    public SaveNodeTypeCommandHandler(INodeTypeRepository nodeTypeRepository,
        ILogger<SaveNodeTypeCommandHandler> logger)
    {
        _nodeTypeRepository = nodeTypeRepository;
        _logger = logger;
    }

    public async Task<DataResult<GetNodeDto>> Handle(
        SaveNodeTypeCommand request,
        CancellationToken cancellationToken
    )
    {
        var nodeTypeToPut = request.Dto.ToDomain();
        await _nodeTypeRepository.Put(nodeTypeToPut);
        await _nodeTypeRepository.SaveChangesAsync();

        var nodeType = await _nodeTypeRepository.GetNodeTypeByIdAsync(nodeTypeToPut.Id);
        if (nodeType is null)
            return DataResult<GetNodeDto>.Failure(new Failure(500, "Failed to save node type"));

        var dto = GetNodeDto.FromDomain(nodeType);
        return DataResult<GetNodeDto>.Success(dto);
    }
}