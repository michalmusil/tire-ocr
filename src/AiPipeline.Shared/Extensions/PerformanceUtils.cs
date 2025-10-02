using System.Diagnostics;

namespace TireOcr.Shared.Extensions;

public static class PerformanceUtils
{
    public static async Task<TimeSpan> PerformTimeMeasuredTask(Func<Task> runTask)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        await runTask();
        stopWatch.Stop();

        var timeTaken = stopWatch.Elapsed;
        return timeTaken;
    }

    public static async Task<(TimeSpan, T)> PerformTimeMeasuredTask<T>(Func<Task<T>> runTask)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        var result = await runTask();
        stopWatch.Stop();

        var timeTaken = stopWatch.Elapsed;
        return (timeTaken, result);
    }
}