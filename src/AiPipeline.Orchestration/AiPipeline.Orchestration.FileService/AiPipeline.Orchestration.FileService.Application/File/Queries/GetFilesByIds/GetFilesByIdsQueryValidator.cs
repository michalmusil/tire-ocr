using FluentValidation;
using TireOcr.Shared.Extensions;

namespace AiPipeline.Orchestration.FileService.Application.File.Queries.GetFilesByIds;

public class GetFilesByIdsQueryValidator : AbstractValidator<GetFilesByIdsQuery>
{
    public GetFilesByIdsQueryValidator()
    {
        RuleFor(x => x.UserId.ToString())
            .IsGuid();
    }
}