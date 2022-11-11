namespace Rio.Extensions;

public static class DictionaryExtension
{
    public static bool AddIfNotContainsKey<TKey,TValue>(this IDictionary<TKey,TValue> @this,TKey key,TValue value)
    {
        if(!@this.ContainsKey(key))
        {
            @this.Add(key, value);
            return true;
        }

        return false;
    }
}

