using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.Orchestration.Runner.Application.File.Queries.GetFileWithDataById;

public class GetFileWithDataByIdQueryValidator : AbstractValidator<GetFileWithDataByIdQuery>
{
    public GetFileWithDataByIdQueryValidator()
    {
        RuleFor(q => q.Id.ToString())
            .IsGuid();
        
        RuleFor(q => q.UserId.ToString())
            .IsGuid();
    }
}