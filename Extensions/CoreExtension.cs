using System.Collections.Concurrent;
using System.ComponentModel;

namespace Rio.Common;

public static class CoreExtension
{
    #region Object

    /// <summary>
    /// An object extension method that converts the @this to an or default.
    /// </summary>
    /// <typeparam name="T">The Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <returns>A T.</returns>
    public static T? AsOrDefault<T>(this object? @this)
    {
        if(@this is null)
        {
            return default;
        }
        try
        {
            return (T?)@this;
        }
        catch(Exception)
        {
            return default;
        }
    }

    /// <summary>
    /// An object extension method that converts the @this to an or default.
    /// </summary>
    /// <typeparam name="T">Generic type parameter</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>A T.</returns>
    public static T AsOrDefault<T>(this object? @this,T defaultValue)
    {
        if(@this is null)
        {
            return defaultValue;
        }
        try
        {
            return (T)@this;
        }
        catch(Exception)
        {
            return defaultValue;
        }
    }

    public static T AsOrDefault<T>(this object? @this,Func<T> defaultValueFactory)
    {
        if(@this is null)
        {
            return defaultValueFactory();
        }
        try
        {
            return (T)@this;
        }
        catch(Exception)
        {
            return defaultValueFactory();
        }
    }

    /// <summary>
    /// A System.Object extension method that toes the given this.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">this.</param>
    /// <returns></returns>
    public static T To<T>(this object? @this)
    {
#nullable disable

        if(@this==null || @this==DBNull.Value)
        {
            return (T)(object)null;
        }
#nullable restore

        var targetType = typeof(T).Unwrap();
        var sourceType = @this.GetType().Unwrap();
        if(sourceType==targetType)
        {
            return (T)@this;
        }
        var converter= TypeDescriptor.GetConverter(sourceType);
        if(converter.CanConvertFrom(targetType))
        {
            return (T)converter.ConvertTo(@this, targetType)!;
        }

        converter = TypeDescriptor.GetConverter(targetType);
        if(converter.CanConvertFrom(sourceType))
        {
            return (T)converter.ConvertFrom(@this)!;
        }

        return (T)Convert.ChangeType(@this, targetType);
    }

    public static object? To(this object? @this,Type type)
    {
        if (@this == null||@this==DBNull.Value)
        {
            return null;
        }

        var targetType = type.Unwrap();
        var sourceType = @this.GetType().Unwrap();

        if(sourceType==targetType)
        {
            return @this;
        }

        var converter = TypeDescriptor.GetConverter(sourceType);
        if(converter.CanConvertTo(targetType))
        {
            return converter.ConvertTo(@this, targetType);
        }

        converter = TypeDescriptor.GetConverter(targetType);
        if(converter.CanConvertFrom(sourceType))
        {
            return converter.ConvertFrom(sourceType);
        }

        return Convert.ChangeType(@this, targetType);
    }

    /// <summary>
    /// A System.Object extension method tha converts this object to an or default
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="this"></param>
    /// <param name="defaultValueFactory"></param>
    /// <returns></returns>
    public static T ToOrDefault<T>(this object? @this,Func<object?,T> defaultValueFactory)
    {
        try
        {
            return (T)@this.To(typeof(T))!;
        }
        catch(Exception)
        {
            return defaultValueFactory(@this);
        }
    }

    public static T ToOrDefault<T>(this object? @this,Func<T> defaultValueFactory)
    {
        return @this.ToOrDefault(_ => defaultValueFactory());
    }

    public static object? ToOrDefault(this object? @this,Type type)
    {
        Guard.NotNull(type,nameof(type));
        try
        {
            return @this.To(type);
        }
        catch(Exception)
        {
            return type.GetDefaultValue();
        }
    }

    public static T? ToOrDefault<T>(this object? @this)
    {
        return @this.ToOrDefault(_ => default(T));
    }

    public static T ToOrDefault<T>(this object? @this,T defaultValue)
    {
        return @this.ToOrDefault(_=>defaultValue);
    }

    public static bool IsAssignableFrom<T>(this object @this)
    {
        var type = @this.GetType();
        return type.IsAssignableFrom(typeof(T));
    }

    public static bool IsAssignableFrom(this object @this,Type targetType)
    {
        var type = @this.GetType();
        return type.IsAssignableFrom(targetType);
    }

    public static T Chain<T>(this T @this,Action<T>? action)
    {
        action?.Invoke(@this);

        return @this;
    }

    public static T? NullIf<T>(this T @this,Func<T,bool>? predicate)where T : class
    {
        if(predicate?.Invoke(@this)==true)
        {
            return default;
        }
        return @this;
    }

    public static TResult? GetValueOrDefault<T,TResult>(this T @this,Func<T,TResult> func)
    {
        try
        {
            return func.Invoke(@this);
        }
        catch(Exception)
        {
            return default;
        }
    }

    public static TResult GetValueOrDefault<T,TResult>(this T @this,Func<T,TResult> func,TResult defaultValue)
    {
        try
        {
            return func.Invoke(@this);
        }
        catch(Exception)
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// A TType extension method that tries.
    /// </summary>
    /// <typeparam name="TType">Type of the type.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="tryFunction">The try function.</param>
    /// <param name="catchValue">The catch value.</param>
    /// <returns>A TResult</returns>
    public static TResult Try <TType,TResult>(this TType @this,Func<TType,TResult> tryFunction,TResult catchValue)
    {
        try
        {
            return tryFunction(@this);
        }
        catch(Exception)
        {
            return catchValue;
        }
    }

    public static TResult Try<TType, TResult>(this TType @this, Func<TType, TResult> tryFunction, Func<TType, TResult> catchValueFactory)
    {
        try
        {
            return tryFunction.Invoke(@this);
        }
        catch(Exception)
        {
            return catchValueFactory.Invoke(@this);
        }
    }

    public static bool Try<TType,TResult>(this TType @this,Func<TType,TResult> tryFunction,out TResult? result)
    {
        try
        {
            result = tryFunction.Invoke(@this);
            return true;
        }
        catch(Exception)
        {
            result = default;
            return false;
        }
    }

    public static bool Try<TType,TResult>(this TType @this,Func<TType,TResult> tryFunction,TResult catchValue,out TResult result)
    {
        try
        {
            result = tryFunction(@this);
            return true;
        }
        catch(Exception)
        {
            result = catchValue;
            return false;
        }
    }

    public static bool Try<TType, TResult>(this TType @this, Func<TType, TResult> tryFunction, Func<TType, TResult> catchValueFactory, out TResult result)
    {
        try
        {
            result = tryFunction(@this);
            return true;
        }
        catch (Exception)
        {
            result = catchValueFactory(@this);
            return false;
        }
    }

    public static bool Try<TType>(this TType @this,Action<TType> tryAction)
    {
        try
        {
            tryAction(@this);
            return true;
        }
        catch(Exception)
        {
            return false;
        }
    }

    public static bool Try<TType>(this TType @this, Action<TType> tryAction,Action<TType> catchAction)
    {
        try
        {
            tryAction(@this);
            return true;
        }
        catch (Exception)
        {
            catchAction(@this);
            return false;
        }
    }

    /// <summary>
    /// A T extension method that check  if the value is  between inclusively the minValue and maxValue.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="minValue">The minimum value.</param>
    /// <param name="maxValue">The maximum value.</param>
    /// <returns>true if the value is between inclusively the minValue and maxValue, other false.</returns>
    public static bool InRange<T>(this T @this, T minValue,T maxValue)where T:IComparable<T>
    {
        return @this.CompareTo(minValue) >= 0 && @this.CompareTo(maxValue) <= 0;
    }

    /// <summary>
    /// A T extension method that query if 'source' is the default value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsDefault<T>(this T source)
    {
        return source is null || source.Equals(default(T));
    }

    /// <summary>
    /// An object extension method than converts the @this to string or return an empty string if the value is null.
    /// </summary>
    /// <param name="this"></param>
    /// <returns></returns>
    public static string ToSafeString(this object? @this) => $"{@this}";

    #endregion Object

    #region Type

    /// <summary>
    /// GetUnderlyingType if nullable else return self.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Type Unwrap(this Type type) 
        => Nullable.GetUnderlyingType(Guard.NotNull(type, nameof(type))) ?? type;


    public static Type? GetUnderlyingType(this Type? type) 
        => Nullable.GetUnderlyingType(Guard.NotNull(type, nameof(type)));


    private static readonly ConcurrentDictionary<Type, object> DefaultValues = new();

    public static object? GetDefaultValue(this Type type)
    {
        Guard.NotNull(type, nameof(type));
        return type.IsValueType && type != typeof(void) ? DefaultValues.GetOrAdd(type, Activator.CreateInstance) : null;

    }

    public static T? CreateInstance<T>(this Type @this, params object?[]? args) => (T?)Activator.CreateInstance(@this, args);

    public static bool HasEmptyConstructor(this Type type) => Guard.NotNull(type, nameof(type)).GetConstructors().Any(c => c.GetParameters().Length == 0);

    public static bool IsNullableType(this Type type)
    {
        Guard.NotNull(type, nameof(type));
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    #endregion Type
}
