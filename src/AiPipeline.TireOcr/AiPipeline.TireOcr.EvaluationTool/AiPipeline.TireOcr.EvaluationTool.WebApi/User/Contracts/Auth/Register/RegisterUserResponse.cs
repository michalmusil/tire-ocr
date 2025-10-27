using AiPipeline.TireOcr.EvaluationTool.Application.User.Dtos;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.User.Contracts.Auth.Register;

public record RegisterUserResponse(
    UserDto User
);