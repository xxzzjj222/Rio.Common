
namespace Rio.Common.Extensions;

public static class PropertiesExtension
{
    public static T? GetProperty<T>(this IDictionary<string,object?> properties, string key)
    {
        return Guard.NotNull(properties).TryGetValue(key, out var value) ? (T?)value : default;
    }

    public static void SetProperty<T>(this IDictionary<string,object?> properties,string key,T value)
    {
        Guard.NotNull(properties)[key] = value;
    }
}
