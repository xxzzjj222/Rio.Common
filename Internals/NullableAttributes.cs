//Copyright(c) Rio. All rights reserved
//Licensed under the Apache license

#if NETSTANDARD2_0

// https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Diagnostics/CodeAnalysis/NullableAttributes.cs
namespace System.Diagnostics.CodeAnalysis;


/// <summary>
/// Specifies that an output will not be null even if the corresponding type allow it. Specifies than an input argument was not null when the call returns.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
internal sealed class NotNullAttribute : Attribute
{

}


internal sealed class NotNullWhenAttribute : Attribute
{
    /// <summary>
    /// Initializes the attribute with the specified return value condition
    /// </summary>
    /// <param name="returnValue">The return value condition. If method returns this value, the associated parameter will not be null.</param>
    public NotNullWhenAttribute(bool returnValue)=>ReturnValue = returnValue;

    public bool ReturnValue { get; }
}

#endif