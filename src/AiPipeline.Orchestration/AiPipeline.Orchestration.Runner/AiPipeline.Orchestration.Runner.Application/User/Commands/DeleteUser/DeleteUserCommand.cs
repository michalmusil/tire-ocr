using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.User.Commands.DeleteUser;

public record DeleteUserCommand(Guid DeletingUserId, Guid UserToDeleteId) : ICommand;