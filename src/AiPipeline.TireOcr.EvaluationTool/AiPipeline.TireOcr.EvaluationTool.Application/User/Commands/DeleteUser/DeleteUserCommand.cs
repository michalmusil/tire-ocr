using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Commands.DeleteUser;

public record DeleteUserCommand(Guid DeletingUserId, Guid UserToDeleteId) : ICommand;