using System;

namespace Rio.Extensions;

public static class TypeExtension
{
    private static readonly Type[] BasicTypes =
    {
        typeof(bool),

        typeof(sbyte),
        typeof(byte),
        typeof(int),
        typeof(uint),
        typeof(short),
        typeof(ushort),
        typeof(long),
        typeof(ulong),
        typeof(float),
        typeof(double),
        typeof(decimal),

        typeof(Guid),
        typeof(DateTime),//IsPrimitive:False
        typeof(TimeSpan),//IsPrimitive:False
        typeof(DateTimeOffset),

        typeof(char),
        typeof(string)//IsPrimitive:False
    };

    public static bool IsBasicType(this Type type)
    {
        var unWrappedType = type.Unwrap();
        return unWrappedType.IsEnum || BasicTypes.Contains(unWrappedType);
    }
}

