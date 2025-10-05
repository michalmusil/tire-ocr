namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Extensions;

public static class ListExtension
{
    public static List<List<T>> GetSubLists<T>(this List<T> list, int sublistSize)
    {
        return list
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / sublistSize)
            .Select(x => x.Select(v => v.Value).ToList())
            .ToList();
    }

    public static void AddIfNotNull<T>(this List<T> list, T? value)
    {
        if (value is not null)
            list.Add(value);
    }
}