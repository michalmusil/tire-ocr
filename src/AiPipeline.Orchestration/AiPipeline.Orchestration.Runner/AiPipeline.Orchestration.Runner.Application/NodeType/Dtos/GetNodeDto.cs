namespace AiPipeline.Orchestration.Runner.Application.NodeType.Dtos;

public record GetNodeDto(
    string Id,
    List<GetNodeProcedureDto> Procedures
)
{
    public static GetNodeDto FromDomain(Domain.NodeTypeAggregate.NodeType domain)
    {
        return new GetNodeDto
        (
            Id: domain.Id,
            Procedures: domain.AvailableProcedures
                .Select(GetNodeProcedureDto.FromDomain)
                .Where(dto => dto is not null)
                .ToList()!
        );
    }
}