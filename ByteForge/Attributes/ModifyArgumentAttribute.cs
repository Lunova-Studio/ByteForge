using System;
using System.Collections.Generic;
using System.Text;

namespace ByteForge.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ModifyArgumentAttribute : Attribute
{
    public int ArgumentIndex { get; }
    public int CallingIndex { get; }
    public Type Type { get; }
    public string Name { get; }
    public Type? ReturnType { get; }
    public Type[] Parameters { get; }
    public ModifyArgumentAttribute(int argumentIndex, int callingIndex, Type type, string name, Type? returnType, params Type[] parameters)
    {
        ArgumentIndex = argumentIndex;
        CallingIndex = callingIndex;
        Type = type;
        Name = name;
        ReturnType = returnType;
        Parameters = parameters;
    }
}
