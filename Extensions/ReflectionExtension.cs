using Rio.Common;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Rio.Extensions;

public static class ReflectionExtension
{
    public static Func<T,object?>? GetValueGetter<T>(this PropertyInfo propertyInfo)
    {
        return StrongTypedCache<T>.PropertyValueGetters.GetOrAdd(propertyInfo, prop =>
        {
            if (!prop.CanRead)
                return null;

            var instance = Expression.Parameter(typeof(T), "i");
            var property = Expression.Property(instance, prop);
            var convert = Expression.TypeAs(property, typeof(T));
            return (Func<T, object>)Expression.Lambda(convert, instance).Compile();
        });
    }

    public static Func<object,object?>? GetValueGetter(this PropertyInfo propertyInfo)
    {
        return CacheUtil.PropertyValueGetters.GetOrAdd(propertyInfo, prop =>
        {
            if (!prop.CanRead)
                return null;

            Debug.Assert(propertyInfo.DeclaringType != null);

            var instance = Expression.Parameter(typeof(object), "obj");
            var getterCall = Expression.Call(propertyInfo.DeclaringType!.IsValueType
                ? Expression.Unbox(instance, propertyInfo.DeclaringType)
                : Expression.Convert(instance, propertyInfo.DeclaringType), prop.GetGetMethod()!);
            var castToObject = Expression.Convert(getterCall, typeof(object));
            return (Func<object, object>)Expression.Lambda(castToObject, instance).Compile();
        });
    }

    public static Action<object,object?>? GetValueSetter(this PropertyInfo propertyInfo)
    {
        Guard.NotNull(propertyInfo, nameof(propertyInfo));
        return CacheUtil.PropertyValueSetters.GetOrAdd(propertyInfo, prop =>
        {
            if (!prop.CanWrite)
                return null;

            var obj = Expression.Parameter(typeof(object), "o");
            var value = Expression.Parameter(typeof(object));

            var expr =
            Expression.Lambda<Action<object, object?>>(
                Expression.Call(
                    propertyInfo.DeclaringType!.IsValueType
                        ? Expression.Unbox(obj, propertyInfo.DeclaringType)
                        : Expression.Convert(value, propertyInfo.DeclaringType),
                    propertyInfo.GetSetMethod()!,
                    Expression.Convert(value, propertyInfo.PropertyType)),
                obj, value);
            return expr.Compile();
        });
    }

    public static FieldInfo GetField<T>(this T @this,string name)
    {
        return CacheUtil.GetTypeFields(typeof(T)).FirstOrDefault(_=>_.Name==name);
    }

    public static FieldInfo GetField<T>(this T @this,string name,BindingFlags bindingFlags)
    {
        if (@this is null)
        {
            throw new ArgumentNullException(nameof(@this));
        }

        return @this.GetType().GetField(name, bindingFlags);
    }

    public static FieldInfo[] GetFields(this object @this)
    {
        return CacheUtil.GetTypeFields(Guard.NotNull(@this, nameof(@this)).GetType());
    }

    public static FieldInfo[] GetFields(this object @this,BindingFlags bindingFlags)
    {
        return @this.GetType().GetFields(bindingFlags);
    }

    public static object? GetFieldValue<T>(this T @this,string fieldName)
    {
        return @this.GetField(fieldName)?.GetValue(@this);
    }

    /// <summary>
    /// A T extension method that query if '@this' is enum
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <returns>true if enum, false if not.</returns>
    public static bool IsEnum<T>(this T @this) => typeof(T).IsEnum;

    public static bool IsValueTuple<T>(this T t) => typeof(T).IsValueTuple();

    public static bool IsValueType<T>(this T t) => typeof(T).IsValueType;
}

