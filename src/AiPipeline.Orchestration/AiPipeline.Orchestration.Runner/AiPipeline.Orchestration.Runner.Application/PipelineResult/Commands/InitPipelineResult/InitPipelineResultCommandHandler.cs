using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Commands.InitPipelineResult;

public class InitPipelineResultCommandHandler : ICommandHandler<InitPipelineResultCommand, GetPipelineResultDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InitPipelineResultCommandHandler> _logger;

    public InitPipelineResultCommandHandler(IUnitOfWork unitOfWork,
        ILogger<InitPipelineResultCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<DataResult<GetPipelineResultDto>> Handle(
        InitPipelineResultCommand request,
        CancellationToken cancellationToken
    )
    {
        var newResult = new Domain.PipelineResultAggregate.PipelineResult(request.PipelineId);
        var validationResult = newResult.Validate();
        if (validationResult.IsFailure)
            return DataResult<GetPipelineResultDto>.Failure(validationResult.Failures);

        await _unitOfWork.PipelineResultEntityRepository.Add(newResult);
        await _unitOfWork.SaveChangesAsync();

        var dto = GetPipelineResultDto.FromDomain(newResult);
        return DataResult<GetPipelineResultDto>.Success(dto);
    }
}