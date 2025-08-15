using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.Orchestration.FileService.Application.File.Queries.GetFileById;

public class GetFileByIdQueryValidator : AbstractValidator<GetFileByIdQuery>
{
    public GetFileByIdQueryValidator()
    {
        RuleFor(q => q.Id.ToString())
            .IsGuid();

        RuleFor(q => q.UserId.ToString())
            .IsGuid();
    }
}