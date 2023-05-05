using Rio.Extensions;
using System.Reflection;

namespace Rio.Common.Helpers;

public static class ActivatorHelper
{
    public static void FindApplicableConstructor(Type instanceType, Type[] argumentTypes,out ConstructorInfo? matchingConstructor,out int?[]? parameterMap)
    {
        matchingConstructor = null;
        parameterMap = null;

        if(!TryFincMatchingConstructor(instanceType,argumentTypes,ref matchingConstructor,ref parameterMap))
        {
            var message = $"A suitable constructor for type '{instanceType}' could not be located. Ensure the type is concrete and services are registered for all parameters of a public constructor.";
            throw new InvalidOperationException(message);
        }
    }

    private static bool TryFincMatchingConstructor(Type instanceType, Type[] argumentTypes, ref ConstructorInfo? matchingConstructor, ref int?[]? parameterMap)
    {
        foreach (var constructor in instanceType.GetTypeInfo().DeclaredConstructors)
        {
            if(constructor.IsStatic || !constructor.IsPublic)
            {
                continue;
            }

            if(TryCreateParameterMap(constructor.GetParameters(), argumentTypes, out var tempParameterMap))
            {
                if(matchingConstructor!=null)
                {
                    throw new InvalidOperationException($"Mutiple constructor accepting all given argument types have been found in type '{instanceType}'. There should only be one applicable constructor.");
                }

                matchingConstructor = constructor;
                parameterMap = tempParameterMap;
            }
        }

        return matchingConstructor != null;
    }

    private static bool TryCreateParameterMap(ParameterInfo[] constructorParameters, Type[] argumentTypes,out int?[] parameterMap)
    {
        parameterMap=new int?[constructorParameters.Length];

        for(var i=0;i<argumentTypes.Length;i++)
        {
            var foundMatch = false;
            var givenParameter = argumentTypes[i];

            for(var j=0;j<constructorParameters.Length;j++)
            {
                if (parameterMap[j]!=null)
                {
                    continue;
                }
                if (constructorParameters[j].IsAssignableFrom(givenParameter))
                {
                    foundMatch = true;
                    parameterMap[j] = i;
                    break;
                }
            }

            if(!foundMatch)
            {
                return false;
            }
        }

        return true;
    }
}
