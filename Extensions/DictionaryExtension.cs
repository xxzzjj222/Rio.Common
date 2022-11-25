using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Rio.Extensions;

public static class DictionaryExtension
{
    /// <summary>
    /// 根据Key获取Dictionary中元素
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="this"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static bool TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, out TValue value, TValue defaultValue)
    {
        if (@this.TryGetValue(key, out var result))
        {
            value = result;
            return true;
        }
        else
        {
            value = defaultValue;
            return false;
        }
    }

    /// <summary>
    /// An IDictionary extension method that adds if not contains key
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="this"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool AddIfNotContainsKey<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, TValue value)
    {
        if (!@this.ContainsKey(key))
        {
            @this.Add(key, value);
            return true;
        }

        return false;
    }

    public static bool AddIfNotContainsKey<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, Func<TValue> valueFactory)
    {
        if (!@this.ContainsKey(key))
        {
            @this.Add(key, valueFactory());
            return true;
        }

        return false;
    }

    public static bool AddIfNotContainsKey<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, Func<TKey, TValue> valueFactory)
    {
        if (!@this.ContainsKey(key))
        {
            @this.Add(key, valueFactory(key));
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds a key/value pair to the IDictionary if the key does not alredy exist.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="this"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, TValue value)
    {
        if (!@this.ContainsKey(key))
        {
            @this.Add(key, value);
        }

        return @this[key];
    }

    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, Func<TKey, TValue> valueFactory)
    {
        if (@this.ContainsKey(key))
        {
            @this.Add(key, valueFactory(key));
        }

        return @this[key];
    }


    public static TValue GetOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key,TValue value)
    {
        if(!@this.ContainsKey(key))
        {
            @this.Add(key,value);
        }
        else
        {
            @this[key] = value;
        }

        return @this[key];
    }

    public static TValue GetOrUpdate<TKey,TValue>(this IDictionary<TKey,TValue> @this,TKey key,TValue addValue,Func<TKey,TValue,TValue> updateValueFactory)
    {
        if(!@this.ContainsKey(key))
        {
            @this.Add(key, addValue);
        }
        else
        {
            @this[key] = updateValueFactory(key, @this[key]);
        }

        return @this[key];
    }

    public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
    {
        if (!@this.ContainsKey(key))
        {
            @this.Add(new KeyValuePair<TKey, TValue>(key, addValueFactory(key)));
        }
        else
        {
            @this[key] = updateValueFactory(key, @this[key]);
        }

        return @this[key];
    }

    public static void RemoveIfContainsKey<TKey,TValue>(this IDictionary<TKey,TValue> @this,TKey key)
    {
        if(@this.ContainsKey(key))
        {
            @this.Remove(key);
        }
    }

    public static SortedDictionary<TKey,TValue> ToSortedDictionary<TKey,TValue>(this IDictionary<TKey,TValue> @this) where TKey:notnull
    {
        return new(@this);
    }

    public static SortedDictionary<TKey,TValue> ToSortedDictionary<TKey,TValue>(this IDictionary<TKey,TValue> @this,IComparer<TKey> comparer) where TKey:notnull
    {
        return new(@this, comparer);
    }

    public static bool ContainsAnyKey<TKey,TValue>(this IDictionary<TKey,TValue> @this,params TKey[] keys) where TKey:notnull
    {
        foreach(var key in keys)
        {
            if(@this.ContainsKey(key))
            {
                return true;
            }
        }

        return false;
    }

    public static bool ContainsAllKey<TKey,TValue>(this IDictionary<TKey,TValue> @this,params TKey[] keys) where TKey:notnull
    {
        foreach(var key in keys)
        {
            if(!@this.ContainsKey(key))
            {
                return false;
            }
        }

        return true;
    }

    public static NameValueCollection ToNameValueCollecton<TKey,TValue>(this IEnumerable<KeyValuePair<string,string>>? source)
    {
        if(source==null)
        {
            return new NameValueCollection();
        }

        var collection = new NameValueCollection();
        foreach(var item in source)
        {
            if(string.IsNullOrWhiteSpace(item.Key))
            {
                continue;
            }
            collection.Add(item.Key, item.Value);
        }

        return collection;
    }

    public static DbParameter[] ToDbParameters(this IDictionary<string,object> @this,DbCommand command)
    {
        return @this.Select(x =>
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = x.Key;
            parameter.Value = x.Value;
            return parameter;
        }).ToArray();
    }

    public static DbParameter[] ToDbParameter(this IDictionary<string,object> @this,DbConnection connection)
    {
        var command = connection.CreateCommand();

        return @this.Select(x =>
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = x.Key;
            parameter.Value = x.Value;
            return parameter;
        }).ToArray();
    }

    public static DataTable ToDataTable(this IDictionary<string,object> @this)
    {
        if(@this==null)
        {
            throw new ArgumentNullException(nameof(@this));
        }

        var dataTable = new DataTable();
        if(@this.Keys.Count==0)
        {
            return dataTable;
        }

        dataTable.Columns.AddRange(@this.Keys.Select(x => new DataColumn(x, @this[x].GetType())).ToArray());
        foreach(var key in @this.Keys)
        {
            var row = dataTable.NewRow();
            row[key] = @this[key];
            dataTable.Rows.Add(row);
        }

        return dataTable;
    }

    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source) where TKey : notnull => source.ToDictionary(pair => pair.Key, pair => pair.Value);

    public static IEnumerable<KeyValuePair<string,string?>> ToKeyValuePair(this NameValueCollection? collection)
    {
        if(collection==null || collection.Count==0)
        {
            yield break;
        }

        foreach (var key in collection.AllKeys)
        {
            if(string.IsNullOrWhiteSpace(key))
            {
                continue;
            }

            yield return new KeyValuePair<string, string?>(key, collection[key]);
        }
    }

    public static string ToQueryString(this IEnumerable<KeyValuePair<string,string>>? source)
    {
        if(source==null)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();

        foreach (var item in source)
        {
            if (string.IsNullOrWhiteSpace(item.Key))
            {
                continue;
            }
            sb.Append("&");
            sb.Append(item.Key.UrlEncode());
            sb.Append("=");
            if (item.Value.IsNotNullOrEmpty())
            {
                sb.Append(item.Value.UrlEncode());
            }
        }

        return sb.Length > 1 ? sb.ToString(1, sb.Length - 1) : string.Empty;
    }
}

