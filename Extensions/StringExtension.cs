using Rio.Common;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace Rio.Extensions;
    /// <summary>
    /// StringExtension
    /// </summary>
public static class StringExtension
{
    /// <summary>
    /// Encodes a URL string
    /// </summary>
    /// <param name="str">The text to encode</param>
    /// <returns>An encoded string</returns>
    public static string UrlEncode(this string str)
    {
        return HttpUtility.UrlEncode(str);
    }

    public static string HtmlEncode(this string s)
    {
        return HttpUtility.HtmlEncode(s);
    }

    public static string HtmlDecode(this string s)
    {
        return HttpUtility.HtmlDecode(s);
    }

    public static Type GetTypeByTypeName(this string typeName)
    {
        var type=Guard.NotNullOrEmpty(typeName,nameof(typeName))
            .ToLower() switch
        {
            "bool"=>Type.GetType("System.Boolean",true,true),
            "byte"=>Type.GetType("System.Byte",true,true),
            "sbyte"=>Type.GetType("System.Sbyte",true,true),
            "char"=>Type.GetType("System.Char",true,true),
            "decimal"=>Type.GetType("System.Decimal",true,true),
            "double"=>Type.GetType("System.Double",true,true),
            "float"=>Type.GetType("System.Single",true,true),
            "int"=>Type.GetType("System.Int32",true,true),
            "uint"=>Type.GetType("System.UInt32",true,true),
            "long"=>Type.GetType("System.Int64",true,true),
            "ulong"=>Type.GetType("System.UInt64",true,true),
            "short"=>Type.GetType("System.Int16",true,true),
            "ushort"=>Type.GetType("System.UInt16",true,true),
            "string"=>Type.GetType("System.String",true,true),
            "datetime"=>Type.GetType("System.DateTime",true,true),
            "guid"=>Type.GetType("System.Guid",true,true),
            "object"=>Type.GetType("System.Object",true,true),
            _=>Type.GetType(typeName,true,true)
        };
        return Guard.NotNull(type);
    }

    public static T[] SplitArray<T>(this string? str, char[] separators,StringSplitOptions options=StringSplitOptions.None)
    {
        Guard.NotNull(str);
        return str.Split(separators, options)
            .Select(_ => _.To<T>())
            .ToArray();
    }

    public static string? TrimStart(this string? str,string start)
    {
        if(str.IsNullOrEmpty()||start.IsNullOrEmpty())
        {
            return str;
        }
        if(str.StartsWith(start))
        {
            str = str.Substring(start.Length);
        }
        return str;
    }
}
