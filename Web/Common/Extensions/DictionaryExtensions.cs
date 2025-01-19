namespace Web.Common.Extensions;

public static class DictionaryExtensions
{
    public static SortedDictionary<TKey, TElement> ToSortedDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(keySelector);
        ArgumentNullException.ThrowIfNull(elementSelector);

        var result = new SortedDictionary<TKey, TElement>();
        foreach (var x in source)
        {
            result.Add(keySelector(x), elementSelector(x));
        }

        return result;
    }
}