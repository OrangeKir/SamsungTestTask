namespace SamsungTestTask.Infrastructure.Extensions;

public static class EnumerableExtensions
{
    public static (IEnumerable<T> Matches, IEnumerable<T> NotMatches) Fork<T>(this IEnumerable<T> source,
        Func<T, bool> predicate)
    {
        var lookup = source.ToLookup(predicate);
        return (lookup[true], lookup[false]);
    }
}