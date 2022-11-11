namespace Rio.Extensions;

public static class EnumerableExtension
{
    public static string StringJoin<T>(this IEnumerable<T> @this, string separator) => string.Join(separator, @this);
}

