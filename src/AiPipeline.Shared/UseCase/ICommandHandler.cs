using MediatR;
using TireOcr.Shared.Result;

namespace TireOcr.Shared.UseCase;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, TireOcr.Shared.Result.Result>
    where TCommand : ICommand
{
}

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, DataResult<TResponse>>
    where TCommand : ICommand<TResponse>
{
}