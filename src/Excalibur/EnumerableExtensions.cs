namespace Excalibur;

public static class EnumerableExtensions
{
    public static async Task<T[]> ToArrayAsync<T>(this Task<IEnumerable<T>> source)
    {
        var items = await source;

        return items.ToArray();
    }

    public static async Task<List<T>> ToListAsync<T>(this Task<IEnumerable<T>> source)
    {
        var items = await source;

        return items.ToList();
    }
}
