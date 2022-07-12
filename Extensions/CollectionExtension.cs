using Rio.Common.Helpers;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Rio.Common.Extensions
{
    /// <summary>
    /// CollectionExtension
    /// </summary>
    public static class CollectionExtension
    {
        /// <summary>
        /// A NameValueCollection extension method than converts the @this to a dictionary
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>@this as an IDictionary&lt;string,string?&gt;</returns>
        public static IDictionary<string, string?> ToDictionary(this NameValueCollection? @this)
        {
            var dict = new Dictionary<string, string?>();

            if (@this != null)
            {
                foreach (var key in @this.AllKeys)
                {
                    dict.Add(Guard.NotNull(key), @this[key]);
                }
            }

            return dict;
        }

        /// <summary>
        /// 将名值集合转换成字符串，key1=value&amp;key2=value2, k/v会编码
        /// </summary>
        /// <param name="source">数据源</param>
        /// <returns>字符串</returns>
        public static string ToQueryString(this NameValueCollection? source)
        {
            if (source == null || source.Count == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            foreach (var key in source.AllKeys)
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }
                sb.Append("&");
                sb.Append(key.UrlEncode());
                sb.Append("=");
                var val = source.Get(key);
                if (val != null)
                {
                    sb.Append(val.UrlEncode());
                }
            }

            return sb.Length > 0 ? sb.ToString(1, sb.Length - 1) : "";
        }

        /// <summary>
        /// An ICollection&lt;T&gt; extension method that adds only if the value statisfies the predicate.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="value">The value.</param>
        /// <returns>true if it succeed, false if it fails.</returns>
        public static bool AddIf<T>(this ICollection<T> @this, Func<T, bool> predicate, T value)
        {
            if (@this.IsReadOnly) return false;

            if (predicate(value))
            {
                @this.Add(value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// An ICollection%lt;T&gt; extension method that add value if the ICollection doesn't contains it already.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">Thw @this to act on.</param>
        /// <param name="value">Thw value.</param>
        /// <returns></returns>
        public static bool AddIfNotContains<T>(this ICollection<T> @this, T value)
        {
            if (@this.IsReadOnly) return false;

            if (!@this.Contains(value))
            {
                @this.Add(value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// An ICollection&lt;T&gt; extension method that adds a range to 'values'.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <param name="values">A variable-length parameters list containing values.</param>
        public static void AddRange<T>(this ICollection<T> @this, params T[] values)
        {
            if (@this.IsReadOnly)
            {
                return;
            }
            foreach (var value in values)
            {
                @this.Add(value);
            }
        }

        /// <summary>
        /// An ICollection&lt;T&gt; extension method that adds a collection of objects to the end of this collection only
        /// for value who statisfies the predicate
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="values">A variable-length parameters list containing values.</param>
        public static void AddRangeIf<T>(this ICollection<T> @this, Func<T, bool> predicate, params T[] values)
        {
            if (@this.IsReadOnly)
            {
                return;
            }
            foreach (var value in values)
            {
                if (predicate(value))
                {
                    @this.Add(value);
                }
            }
        }

        /// <summary>
        /// An ICollection&lt;T&gt; extension method that adds a range of values that's not already in the ICollection
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <param name="values">A variable-length parametes list containing values.</param>
        public static void AddRangeIfNotContains<T>(this ICollection<T> @this, params T[] values)
        {
            if (@this.IsReadOnly)
            {
                return;
            }
            foreach (var value in values)
            {
                if (!@this.Contains(value))
                {
                    @this.Add(value);
                }
            }
        }

        /// <summary>
        /// An ICollection&lt;T&gt; extension method that query if '@this' contains all values.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <param name="values">A variable-length parameters list containing values.</param>
        /// <returns>true if it succeeds, false if it fails</returns>
        public static bool ContainsAll<T>(this ICollection<T> @this, params T[] values)
        {
            return values.All(@this.Contains);
        }

        /// <summary>
        /// An ICollection&;lt;T&gt; extension method that query if '@this' contains any value.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <param name="values">A variable-length parameters list containing values.</param>
        /// <returns>true if it succeeds, false if it fails</returns>
        public static bool ContainsAny<T>(this ICollection<T> @this, params T[] values)
        {
            return values.Any(@this.Contains);
        }

        /// <summary>
        /// An ICollection&lt;&gt; extension method that query if the collection is null or is empty. 
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <returns>true if null or empty, false if not.</returns>
        public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this ICollection<T>? @this)
        {
            return @this == null || @this.Count == 0;
        }

        /// <summary>
        /// An ICollection&lt;T&gt; extension method than queries if the collection is not (null or is empty).
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <returns>true if the collection is not (null or empty), false if not.</returns>
        public static bool HasValue<T>([NotNullWhen(true)] this ICollection<T>? @this)
        {
            return @this is { Count: > 0 };
        }

        /// <summary>
        /// An ICollection&lt;T&gt; extension method that removes value that statisfy the predicate.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <param name="predicate">The predicate.</param>
        public static void RemoveWhere<T>(this IList<T> @this, Func<T, bool> predicate)
        {
            if (@this.IsReadOnly || @this.Count == 0) return;

            for (var i = @this.Count - 1; i >= 0; i--)
            {
                if (predicate(@this[i]))
                {
                    @this.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// An ICollection&lt;T&gt; extension method that random the collection.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <returns>A random collection.</returns>
        public static IEnumerable<T> GetRandomList<T>(this IList<T> @this)
        {
            return Enumerable.Range(0, @this.Count)
                .OrderBy(_ => SecurityHelper.Random.Next(@this.Count))
                .Select(i => @this[i]);
        }

#if NET6_0_OR_GREATER
        // https://github.com/more-itertools/more-itertools/blob/master/more_itertools/more.py#L3149
        //def set_partitions_helper(L, k):
        //n = len(L)
        //if k == 1:
        //    yield [L]
        //elif n == k:
        //    yield [[s] for s in L]
        //else:
        //    e, *M = L
        //    for p in set_partitions_helper(M, k - 1):
        //        yield [[e], *p]
        //    for p in set_partitions_helper(M, k):
        //        for i in range(len(p)):
        //            yield p[:i] + [[e] + p[i]] + p[i + 1 :]
        public static IEnumerable<T[][]> Partitions<T>(this T[] array, int batch)
        {
            if (batch <= 0 || array.Length < batch)
            {
                throw new ArgumentException("Invalid bathc size", nameof(batch));
            }
            if (batch == 1)
            {
                yield return new[] { array };
            }
            else if (batch == array.Length)
            {
                yield return array.Select(x => new[] { x }).ToArray();
            }
            else
            {
                var e = array[0];
                var m = array[1..];
                foreach (var p in Partitions(m, batch - 1))
                {
                    yield return new[]
                    {
                        new[]{e}
                    }.Concat(p).ToArray();
                }
                foreach(var p in Partitions(m,batch))
                {
                    for (var i = 0; i < p.Length; i++)
                    {
                        yield return p[..i]
                            .Concat(new[] { new[] { e }.Concat(p[i]).ToArray() })
                            .Concat(p[(i+1)..])
                            .ToArray();
                    }
                }
            }
        }
#endif
    }
}
