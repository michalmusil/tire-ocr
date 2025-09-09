using FluentValidation;
using MediatR;
using TireOcr.Shared.Result;

namespace TireOcr.Shared.Behaviors;

public class ValidationPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result.Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var failures = _validators
            .Select(v => v.Validate(request))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .Select(f => new Failure(422, $"{f.PropertyName}: {f.ErrorMessage}"))
            .Distinct()
            .ToArray();

        if (!failures.Any())
        {
            return await next();
        }

        return CreateFailureResult<TResponse>(failures);
    }

    private static T CreateFailureResult<T>(Failure[] failures)
        where T : Result.Result
    {
        var responseType = typeof(T);
        // Result
        if (responseType == typeof(Result.Result))
        {
            return (T) Result.Result.Failure(failures);
        }

        // DataResult
        var failureDataResult = typeof(DataResult<>)
            .GetGenericTypeDefinition()
            .MakeGenericType(responseType.GenericTypeArguments[0])
            .GetMethod(nameof(Failure), new[] { failures.GetType() })!
            .Invoke(null, new object?[] { failures });

        return (T)failureDataResult!;
    }
}