using MediatR;
using TireOcr.Shared.Result;

namespace TireOcr.Shared.UseCase;

public interface IQuery : IRequest<TireOcr.Shared.Result.Result>
{
}

public interface IQuery<TResponse> : IRequest<DataResult<TResponse>>
{
}