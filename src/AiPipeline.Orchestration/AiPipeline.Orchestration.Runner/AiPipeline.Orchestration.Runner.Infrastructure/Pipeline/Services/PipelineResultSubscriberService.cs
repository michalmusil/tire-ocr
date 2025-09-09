using System.Collections.Concurrent;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Services;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Services;

public class PipelineResultSubscriberService : IPipelineResultSubscriberService
{
    private readonly
        ConcurrentDictionary<Guid,
            List<TaskCompletionSource<DataResult<Domain.PipelineResultAggregate.PipelineResult>>>>
        _completionDictionary;

    public PipelineResultSubscriberService()
    {
        _completionDictionary = new();
    }

    public async Task<DataResult<Domain.PipelineResultAggregate.PipelineResult>> WaitForPipelineResultAsync(
        Guid pipelineId,
        TimeSpan timeout,
        CancellationToken? cancellationToken = null
    )
    {
        if (!_completionDictionary.ContainsKey(pipelineId))
            _completionDictionary.TryAdd(pipelineId, new());

        var successfulCompletionSource =
            new TaskCompletionSource<DataResult<Domain.PipelineResultAggregate.PipelineResult>>(
                TaskCreationOptions.RunContinuationsAsynchronously);
        _completionDictionary[pipelineId].Add(successfulCompletionSource);

        return await AwaitCompletionWithCancellation(
            pipelineId: pipelineId,
            successfulCompletionSource: successfulCompletionSource,
            timeout: timeout,
            cancellationToken: cancellationToken
        );
    }

    public Task<Result> CompleteWithPipelineResultAsync(
        Domain.PipelineResultAggregate.PipelineResult pipelineResult)
    {
        var pipelineId = pipelineResult.PipelineId;
        var completersFound = _completionDictionary.TryGetValue(pipelineId, out var completers);
        if (!completersFound)
            return Task.FromResult(Result.Success());

        var result = DataResult<Domain.PipelineResultAggregate.PipelineResult>.Success(pipelineResult);
        foreach (var completer in completers!)
            completer.SetResult(result);

        _completionDictionary.TryRemove(pipelineId, out _);
        return Task.FromResult(Result.Success());
    }

    public Task<Result> CompleteWithPipelineFailuresAsync(Guid pipelineId, params Failure[] failures)
    {
        var completersFound = _completionDictionary.TryGetValue(pipelineId, out var completers);
        if (!completersFound)
            return Task.FromResult(Result.Success());

        var result = DataResult<Domain.PipelineResultAggregate.PipelineResult>.Failure(failures);
        foreach (var completer in completers)
            completer.SetResult(result);

        _completionDictionary.TryRemove(pipelineId, out _);
        return Task.FromResult(Result.Success());
    }

    private async Task<DataResult<Domain.PipelineResultAggregate.PipelineResult>> AwaitCompletionWithCancellation(
        Guid pipelineId,
        TaskCompletionSource<DataResult<Domain.PipelineResultAggregate.PipelineResult>> successfulCompletionSource,
        TimeSpan timeout,
        CancellationToken? cancellationToken
    )
    {
        var cancelledCompletionSource =
            new TaskCompletionSource<DataResult<Domain.PipelineResultAggregate.PipelineResult>>(
                TaskCreationOptions.RunContinuationsAsynchronously);

        using var timeoutTokenSource = new CancellationTokenSource(timeout);
        using var cancellationTokenSource = cancellationToken is null
            ? timeoutTokenSource
            : CancellationTokenSource.CreateLinkedTokenSource((CancellationToken)cancellationToken,
                timeoutTokenSource.Token);

        await using var cancellationRegistration = cancellationTokenSource.Token.Register(() =>
        {
            if (timeoutTokenSource.IsCancellationRequested)
            {
                var timeoutResult =
                    DataResult<Domain.PipelineResultAggregate.PipelineResult>.Timeout(
                        $"Awaiting result of pipeline '{pipelineId}' timed out after {timeout.TotalSeconds}s");
                cancelledCompletionSource.SetResult(timeoutResult);
            }
            else
            {
                var cancelledResult = DataResult<Domain.PipelineResultAggregate.PipelineResult>.Cancelled(
                    $"Awaiting result of pipeline '{pipelineId}' was cancelled");
                cancelledCompletionSource.SetResult(cancelledResult);
            }

            var sourcesLocated = _completionDictionary.TryGetValue(pipelineId, out var completionSources);
            if (sourcesLocated)
            {
                completionSources?.Remove(successfulCompletionSource);
                if (completionSources?.Count == 0)
                    _completionDictionary.TryRemove(pipelineId, out _);
            }
        });

        var firstCompletion = await Task.WhenAny(successfulCompletionSource.Task, cancelledCompletionSource.Task);
        return await firstCompletion;
    }
}