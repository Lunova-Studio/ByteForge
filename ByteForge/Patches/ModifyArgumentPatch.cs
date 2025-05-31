using ByteForge.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ByteForge.Patches;

public class ModifyArgumentPatch : IPatch
{
    private MethodInfo method { get; set; }

    public ModifyArgumentPatch(MethodInfo method)
    {
        this.method = method;
    }

    public MethodInfo GetMethod()
    {
        return method;
    }
}
