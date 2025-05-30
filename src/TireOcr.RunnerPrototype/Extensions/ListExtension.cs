namespace TireOcr.RunnerPrototype.Extensions;

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
}