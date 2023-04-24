using System.Reflection;
using System.Reflection.Emit;

namespace Rio.Extensions;

public static class ILGeneratorExtension
{
    public static readonly MethodInfo GetMethodFromHandle = ExpressionExtension.GetMethod<Func<RuntimeMethodHandle, RuntimeTypeHandle, MethodBase>>((h1, h2) => MethodBase.GetMethodFromHandle(h1, h2)!);

    public static void EmitMethod(this ILGenerator ilGenerator,MethodInfo method,Type declaringType)
    {
        if(ilGenerator == null) throw new ArgumentNullException(nameof(ilGenerator));

        if(method == null) throw new ArgumentNullException(nameof(method));

        if(declaringType == null) throw new ArgumentNullException(nameof(declaringType));

        ilGenerator.Emit(OpCodes.Ldtoken,method);
        ilGenerator.Emit(OpCodes.Ldtoken, declaringType);
        ilGenerator.Emit(OpCodes.Call, GetMethodFromHandle);
        //ilGenerator.EmitConvertToType()
    }

    //public static void EmitConvertToType(this ILGenerator ilGenerator,Type typeFrom,Type typeTo,bool isCheck=true)
    //{
    //    if(ilGenerator==null) throw new ArgumentNullException(nameof(ilGenerator));

    //    if(typeFrom == null) throw new ArgumentNullException(nameof(typeFrom));

    //    if (typeTo == null) throw new ArgumentNullException(nameof(typeTo));

    //    var nnExprType = typeFrom.Unwrap();
    //    var nnType = typeTo.Unwrap();

    //    if (nnType == nnExprType || nnType.IsEquivalentTo(nnExprType))
    //    {
    //        return;
    //    }

    //    if(typeFrom.IsInterface || typeTo.IsInterface
    //        || typeFrom==typeof(object)
    //        || typeTo==typeof(object)
    //        || typeFrom==typeof(Enum)
    //        || typeFrom==typeof(ValueType))
    //    {
    //        return;
    //    }
    //}
}

internal static class TypeInfoUtils
{
    internal static bool AreEquivalent(Type t1,Type t2)
    {
        return t1 == t2 || t1.IsEquivalentTo(t2);
    }

    internal static Type GetNonNullableType(this Type type)
    {
        if(type.IsNullableType())
        {
            return type.GetGenericArguments()[0];
        }
        return type;
    }

    private static bool IsDelegate(Type type)
    {
        return type.IsSubclassOf(typeof(MulticastDelegate));
    }

    private static bool IsInvariant(Type t)
    {
        return 0 == (t.GenericParameterAttributes & GenericParameterAttributes.VarianceMask);
    }

    private static bool IsCovariant(Type t)
    {
        return 0 == (t.GenericParameterAttributes & GenericParameterAttributes.Covariant);
    }
}

