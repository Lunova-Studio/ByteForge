using ByteForge.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ByteForge.Core.Patches;

public class RewritePatch : IPatch
{
    private MethodInfo method {  get; set; }

    public RewritePatch(MethodInfo method)
    {
        this.method = method;
    }

    public MethodInfo GetMethod()
    {
        return method;
    }
}
