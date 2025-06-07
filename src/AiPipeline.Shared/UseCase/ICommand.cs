using MediatR;
using TireOcr.Shared.Result;

namespace TireOcr.Shared.UseCase;

public interface ICommand : IRequest<TireOcr.Shared.Result.Result>
{
}

public interface ICommand<TResponse> : IRequest<DataResult<TResponse>>
{
}