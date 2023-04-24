using Newtonsoft.Json;
using Rio.Common;

namespace Rio.Extensions;

public static class JsonSerializeExtension
{
    private static readonly JsonSerializerSettings DefaultSerializerSettings = GetDefaultSerializerSettings();

    private static JsonSerializerSettings GetDefaultSerializerSettings() => new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        MissingMemberHandling = MissingMemberHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore
    };

    public static string ToJson(this object? obj) => obj.ToJson(false, null);

    public static string ToJson(this object? obj, bool isConvertToSingleQuotes) => obj.ToJson(isConvertToSingleQuotes, null);

    public static string ToJson(this object? obj, JsonSerializerSettings? settings) => obj.ToJson(false, settings);

    /// <summary>
    /// 讲Object对象转为json数据
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="isConvertToSingleQuotes"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    public static string ToJson(this object? obj, bool isConvertToSingleQuotes, JsonSerializerSettings? settings)
    { 
        if(obj == null)
        {
            return string.Empty;
        }

        var result = JsonConvert.SerializeObject(obj, settings??DefaultSerializerSettings) ?? string.Empty;
        if(isConvertToSingleQuotes)
        {
            result = result.Replace("\"", "'");
        }
        return result;
    }

    public static T JsonToObject<T>(this string jsonString)
        => jsonString.JsonToObject<T>(null);

    public static T JsonToObject<T>(this string jsonString, JsonSerializerSettings? settings)
        => Guard.NotNull(JsonConvert.DeserializeObject<T>(jsonString, settings ?? DefaultSerializerSettings));

    public static string ToJsonOrString(this object? obj)
    {
        if(obj==null)
        {
            return string.Empty;
        }
        if(obj is string str)
        {
            return str;
        }
        if(obj.GetType().IsBasicType())
        {
            return obj.ToString();
        }
        return obj.ToJson();
    }

    public static T StringToType<T>(this string jsonString)
    {
        if(typeof(T)==typeof(string))
        {
            return (T)(object)jsonString;
        }
        if(typeof(T).IsBasicType())
        {
            return jsonString.To<T>();
        }
        return jsonString.StringToType<T>();
    }

    public static T StringToType<T>(this string jsonString,T defaultValue)
    {
        if(typeof(T)==typeof(string))
        {
            return (T)(object)jsonString;
        }
        if(typeof(T).IsBasicType())
        {
            return Guard.NotNull(jsonString.ToOrDefault<T>(defaultValue));
        }
        if(string.IsNullOrWhiteSpace(jsonString))
        {
            return defaultValue;
        }
        try
        {
            return jsonString.JsonToObject<T>();
        }
        catch
        {
            return defaultValue;
        }
    }
}
