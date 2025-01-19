namespace Web.Common.Extensions;

public static class ArrayExtensions
{
    public static void Each<T>(this IEnumerable<T> ie, Action<T, int> action, int start = 0)
    {
        var i = start;
        foreach (var e in ie)
        {
            action(e, i++);
        }
    }
}