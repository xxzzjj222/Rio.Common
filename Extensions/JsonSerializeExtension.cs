using Newtonsoft.Json;

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


    public static string ToJsonOrString(this object? obj)
    {
        if(obj is null)
        {
            return string.Empty;
        }
        if(obj is string str)
        {
            return str;
        }
        if(obj.GetType().IsBasicType())
        {
            return Convert.ToString(obj) ?? string.Empty;
        }
        else
        {
            return obj.ToJson();
        }
    }

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
}
