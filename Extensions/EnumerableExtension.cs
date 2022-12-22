using Rio.Common;
using Rio.Common.Models;
using System.Collections.ObjectModel;

namespace Rio.Extensions;

public static class EnumerableExtension
{
    public static void ForEach<T>(this IEnumerable<T>ts,Action<T> action)
    {
        foreach (var t in ts)
        {
            action(t);
        }
    }

    public static void ForEach<T>(this IEnumerable<T> ts,Action<T,int> action)
    {
        int i = 0;
        foreach(var t in ts)
        {
            action(t, i);
            i++;
        }
    }

    public static async Task ForEachAsync<T>(this IEnumerable<T> ts,Func<T,Task> action)
    {
        foreach(var t in ts)
        {
            await action(t);
        }
    }

    public static async Task ForEachAsync<T>(this IEnumerable<T> ts,Func<T,int,Task> action)
    {
        int i = 0;
        foreach(var t in ts)
        {
            await action(t, i);
            i++;
        }
    }

    public static ReadOnlyCollection<T> AsReadOnly<T>(this IEnumerable<T> @this)
    {
        return Array.AsReadOnly(@this.ToArray());
    }

    public static bool HasValue<T>(this IEnumerable<T> @this)
    {
        return @this != null && @this.Any();
    }

    public static string StringJoin<T>(this IEnumerable<T> @this, string separator) => string.Join(separator, @this);


#if NETSTANDARD2_0

    public static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> @this,TSource value)
    {
        yield return value;

        foreach(var element in @this)
        {
            yield return element;
        }    
    }

    public static IEnumerable<TSource> Append<TSource>(this IEnumerable<TSource> @this,TSource value)
    {
        foreach(var element in @this)
        {
            yield return element;
        }

        yield return value;
    }

#endif

    #region Split
    public static IEnumerable<T[]> Split<T>(this IEnumerable<T> source,int batchSize)
    {
        using var enumerator = source.GetEnumerator();
        while(enumerator.MoveNext())
        {
            yield return Split(enumerator, batchSize).ToArray();
        }
    }

    public static IEnumerable<T> Split<T>(this IEnumerator<T> enumerator,int batchSize)
    {
        do
        {
            yield return enumerator.Current;
        } while (enumerator.MoveNext() && --batchSize > 0);
    }

    #endregion Split

    public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, Func<T, bool> predict, bool condition)
        => condition ? Guard.NotNull(source, nameof(source)).Where(predict) : source;

    public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, Func<T, bool> predict, Func<bool> condition)
        => condition() ? Guard.NotNull(source, nameof(source)).Where(predict) : source;

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class
        => Guard.NotNull(source, nameof(source)).Where(_ => _ != null)!;

    public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Func<T?, T?, bool> comparer) where T : class
        => source.Distinct();

    private sealed class DynamicEqualityComparer<T> : IEqualityComparer<T>
        where T:class
    {
        private readonly Func<T?, T?, bool> _func;

        public DynamicEqualityComparer(Func<T?,T?,bool> func)
        {
            _func = func;
        }

        public bool Equals(T x, T y) => _func(x, y);

        public int GetHashCode(T obj) => 0; //Force Equals
    }

    #region Linq

    public static IEnumerable<TResult> LeftJoin<TOuter,TInner,TKey,TResult>(this IEnumerable<TOuter> outer,IEnumerable<TInner> inner,
        Func<TOuter,TKey> outerKeySelector,Func<TInner,TKey> innerKeySelector,
        Func<TOuter,TInner?,TResult> resultSelector)
    {
        return outer
            .GroupJoin(inner, outerKeySelector, innerKeySelector, (outerObj, inners) => new
            {
                outerObj,
                inners = inners.DefaultIfEmpty()
            })
            .SelectMany(a => a.inners.Select(innerObj => resultSelector(a.outerObj, innerObj)));
    }

    #endregion Linq

    #region ToPagedList

    public static IListResultWithTotal<T> ToListResultWithTotal<T>(this IEnumerable<T> data, int totalCount)
        => new ListResultWithTotal<T>
        {
            TotalCount = totalCount,
            Data = data is IReadOnlyList<T> dataList ? dataList : data.ToArray()
        };

    public static IPageListResult<T> ToPageList<T>(this IEnumerable<T> data, int pageNumber, int pageSize, int totalCount)
        => new PageListResult<T>
        {
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Data = data is IReadOnlyList<T> dataList ? dataList : data.ToArray()
        };

    public static IPageListResult<T> ToPageListResult<T>(this IReadOnlyList<T> data, int pageNumber, int pageSize, int totalCount)
        => new PageListResult<T>
        {
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Data = data
        };

    #endregion ToPagedList

}

