using MediatR;
using TireOcr.Shared.Result;

namespace TireOcr.Shared.UseCase;

public interface IQueryHandler<in TQuery> : IRequestHandler<TQuery, TireOcr.Shared.Result.Result>
    where TQuery : IQuery<TireOcr.Shared.Result.Result>
{
}

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, DataResult<TResponse>>
    where TQuery : IQuery<TResponse>
{
}