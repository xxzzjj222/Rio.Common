using Rio.Common;
using Rio.Common.Helpers;
using System;
using System.ComponentModel;
using System.Reflection;

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
        typeof(string),//IsPrimitive:False

        //typeof(object),// IsPrimitive:False
    };

    public static bool IsBasicType(this Type type)
    {
        var unWrappedType = type.Unwrap();
        return unWrappedType.IsEnum || BasicTypes.Contains(unWrappedType);
    }

    public static bool IsBasicType<T>() => typeof(T).IsBasicType();

    public static bool IsValueTuple(this Type type)
        => type.IsValueType && type.FullName?.StartsWith("System.ValueTuple`", StringComparison.OrdinalIgnoreCase) == true;

    public static TypeCode GetTypeCode(this Type type) => Type.GetTypeCode(type);

    public static string GetDescription(this Type type) => type.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty;

    public static bool IsPrimitiveType(this Type type) => (Nullable.GetUnderlyingType(type) ?? type).IsPrimitive;

    public static bool IsPrimitiveType<T>() => typeof(T).IsPrimitiveType();

    public static bool HasNameSpace(this Type type) => Guard.NotNull(type,nameof(type)).Namespace != null;

    public static ConstructorInfo? GetEmptyConstructor(this Type type)
    {
        var constructors = type.GetConstructors();

        var ctor = constructors.OrderBy(x => x.IsPublic ? 0 : x.IsPrivate ? 2 : 1)
            .ThenBy(c => c.GetParameters().Length).FirstOrDefault();

        return ctor?.GetParameters().Length == 0 ? ctor : null;
    }

    public static ConstructorInfo? GetConstructorInfo(this Type type,params Type[] parameterTypes)
    {
        if(parameterTypes==null || parameterTypes.Length==0)
        {
            return GetEmptyConstructor(type);
        }

        ActivatorHelper.FindApplicableConstructor(type, parameterTypes, out ConstructorInfo? matchingConstructor, out int?[]? parameterMap);

        return matchingConstructor;
    }

    public static bool IsAssignableTo<T>(this Type @this)
    {
        if(@this == null)
        {
            throw new ArgumentNullException(nameof(@this));
        }

        return typeof(T).IsAssignableFrom(@this.GetType());
    }

    public static ConstructorInfo? GetMatchingConstrutor(this Type type, params Type[] parameterTypes)
    {
        return type.GetConstructors()
            .FirstOrDefault(c=>c.GetParameters().Select(p=>p.ParameterType).SequenceEqual(parameterTypes));
    }

    public static IEnumerable<Type> GetImplementedInterfaces(this Type type)
    {
        return type.GetTypeInfo().ImplementedInterfaces;
    }
}

