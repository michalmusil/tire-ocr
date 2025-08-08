using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.NodeType.Dtos;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.NodeType.Commands;

public class SaveNodeTypeCommandHandler : ICommandHandler<SaveNodeTypeCommand, GetNodeDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SaveNodeTypeCommandHandler> _logger;

    public SaveNodeTypeCommandHandler(IUnitOfWork unitOfWork,
        ILogger<SaveNodeTypeCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<DataResult<GetNodeDto>> Handle(
        SaveNodeTypeCommand request,
        CancellationToken cancellationToken
    )
    {
        var nodeTypeRepository = _unitOfWork.NodeTypeRepository;
        var nodeTypeToPut = request.Dto.ToDomain();
        await nodeTypeRepository.Put(nodeTypeToPut);
        await _unitOfWork.SaveChangesAsync();

        var nodeType = await nodeTypeRepository.GetNodeTypeByIdAsync(nodeTypeToPut.Id);
        if (nodeType is null)
            return DataResult<GetNodeDto>.Failure(new Failure(500, "Failed to save node type"));

        var dto = GetNodeDto.FromDomain(nodeType);
        return DataResult<GetNodeDto>.Success(dto);
    }
}