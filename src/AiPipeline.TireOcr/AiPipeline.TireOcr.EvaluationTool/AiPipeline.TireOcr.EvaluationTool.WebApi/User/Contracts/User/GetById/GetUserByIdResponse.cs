using AiPipeline.TireOcr.EvaluationTool.Application.User.Dtos;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.User.Contracts.User.GetById;

public record GetUserByIdResponse(
    UserDto User
);